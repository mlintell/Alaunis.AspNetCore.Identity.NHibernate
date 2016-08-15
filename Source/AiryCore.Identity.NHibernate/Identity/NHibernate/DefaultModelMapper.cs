namespace AiryCore.Identity.NHibernate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using AiryCore.Extensions.NHibernate;
    using AiryCore.Identity.Core.Attribute;
    using AiryCore.UserTypes;

    using global::NHibernate.Driver;
    using global::NHibernate.Mapping.ByCode;

    /// <summary>
    /// The default model mapper.
    /// </summary>
    /// <seealso cref="ConventionModelMapper" />
    public class DefaultModelMapper : ConventionModelMapper
    {
        /// <summary>
        /// The suffix (added to the name of the property) used for the name for foreign key fields.
        /// </summary>
        private readonly string _foreignKeyColumnSuffix;

        /// <summary>
        /// The prefix (added before the name of the property) used for the name for foreign key.
        /// </summary>
        private readonly string _foreignKeyNamePrefix;

        /// <summary>
        /// The string inserted between the names of the two tables being linked in a many to many relationship to create the link table name.
        /// </summary>
        private readonly string _manyToManyLinkTableInsert;

        /// <summary>
        /// Creates a new model mapper with the convention naming style using the supplied values.
        /// </summary>
        /// <param name="useDateTimeOffsetWorkaround">true to use the DateTimeOffsetSplitType custom user type as a workaround for databases without DateTimeOffset support, false to use the default DateTimeOffset type.</param>
        /// <param name="useConventionMapping">true to use the convention naming style mapping, false to use the mapping by code base style mapping.</param>
        /// <param name="foreignKeyColumnSuffix">The suffix to use for foreign key columns.</param>
        /// <param name="manyToManyLinkTableInsert">The insert to use between the referenced table names for a link table name.</param>
        /// <param name="foreignKeyNamePrefix">The prefix to use before the foreign key name.</param>
        public DefaultModelMapper(
            bool useDateTimeOffsetWorkaround = false,
            bool useConventionMapping = true,
            string foreignKeyColumnSuffix = "Id",
            string manyToManyLinkTableInsert = "To",
            string foreignKeyNamePrefix = "FK_")
        {
            this._foreignKeyColumnSuffix = foreignKeyColumnSuffix;
            this._manyToManyLinkTableInsert = manyToManyLinkTableInsert;
            this._foreignKeyNamePrefix = foreignKeyNamePrefix;
            this.DeafultMapperSetup();

            if (useDateTimeOffsetWorkaround)
            {
                this.BeforeMapProperty += this.ApplyDateTimeOffsetSplitTypeToDateTimeOffset;
            }

            if (useConventionMapping)
            {
                this.AddNamingConventionsToMapper();
            }
        }

        /// <summary>
        /// Allows the default length for string properties to be changed.
        /// Default value is to use maximum permitted by the database, e.g. VARCHAR(MAX) for Microsoft SQL Server.
        /// </summary>
        public int DefaltStringIdLength { get; set; }

        /// <summary>
        /// Allows the default length for string properties to be changed.
        /// Default value is to use maximum permitted by the database, e.g. VARCHAR(MAX) for Microsoft SQL Server.
        /// </summary>
        public int DefaltStringLength { get; set; }

        /// <summary>
        /// Gets the object type names for a many to many relationship sorted alphabetically.
        /// </summary>
        private static IEnumerable<string> GetManyToManySidesNames(PropertyPath member)
        {
            yield return member.GetRootMemberType().Name;
            yield return member.GetCollectionElementType().Name;
        }

        /// <summary>
        /// Returns if a type is nullable or not.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if the type passed is nullable, otherwise false</returns>
        private static bool IsNullable(Type type)
        {
            if (!type.IsValueType) return true; // Type is a reference type so must be nullable.
            if (Nullable.GetUnderlyingType(type) != null) return true; // Type is a value type of Nullable<T> so must be nullable.
            return false; // Otherwise the type must be a value type, so isn't nullable.
        }

        /// <summary>
        /// Sets the naming conventions that are optional applied to this mapper.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private void AddNamingConventionsToMapper()
        {
            // Add the foreign key column suffix to the foreign key fields.
            this.BeforeMapManyToOne += (inspector, member, customizer) =>
                {
                    string columnName = member.LocalMember.Name + this._foreignKeyColumnSuffix;
                    customizer.Column(columnName);
                    string tableName = member.LocalMember.DeclaringType.Name;
                    string foreignKeyName = $"{this._foreignKeyNamePrefix}{tableName}_{columnName}";
                    customizer.ForeignKey(foreignKeyName);
                };
            this.BeforeMapManyToMany += (inspector, member, customizer) =>
                {
                    string columnName = member.GetCollectionElementType().Name + this._foreignKeyColumnSuffix;
                    customizer.Column(columnName);
                    string tableName = this.GetManyToManyLinkTableName(member);
                    string foreignKeyName = $"{this._foreignKeyNamePrefix}{tableName}_{columnName}";
                    customizer.ForeignKey(foreignKeyName);
                };
            this.BeforeMapJoinedSubclass += (inspector, type, customizer) =>
                {
                    customizer.Key(
                        k =>
                            {
                                string columnName = type.BaseType.Name + this._foreignKeyColumnSuffix;
                                k.Column(columnName);
                                string tableName = type.DeclaringType.Name;
                                string foreignKeyName = $"{this._foreignKeyNamePrefix}{tableName}_{columnName}";
                                k.ForeignKey(foreignKeyName);
                            });
                };

            // Add Collection mapping conventions.
            this.BeforeMapSet += this.BeforeMappingCollectionConvention;
            this.BeforeMapBag += this.BeforeMappingCollectionConvention;
            this.BeforeMapList += this.BeforeMappingCollectionConvention;
            this.BeforeMapIdBag += this.BeforeMappingCollectionConvention;
            this.BeforeMapMap += this.BeforeMappingCollectionConvention;
        }

        /// <summary>
        /// Sets the following conventions:
        /// 1) Foreign key fields are named as the property name suffixed by the value of _foreignKeyColumnSuffix.
        /// 2) Many to Many link tables are named as the object type names sorted alphabetically with the _manyToManyLinkTableInsert inserted inbetween them.
        /// </summary>
        private void BeforeMappingCollectionConvention(
            IModelInspector inspector,
            PropertyPath member,
            ICollectionPropertiesMapper customizer)
        {
            string tableName;
            if (inspector.IsManyToMany(member.LocalMember))
            {
                tableName = this.GetManyToManyLinkTableName(member);
                customizer.Table(tableName);
            }
            else
            {
                tableName = member.GetCollectionElementType().Name;
            }

            string columnName = this.GetKeyColumnName(inspector, member);
            string foreignKeyName = $"{this._foreignKeyNamePrefix}{tableName}_{columnName}";
            customizer.Key(
                k =>
                    {
                        k.Column(columnName);
                        k.ForeignKey(foreignKeyName);
                    });
        }

        /// <summary>
        /// Sets the mapper conventions that are always applied to this mapper.
        /// </summary>
        private void DeafultMapperSetup()
        {
            this.DefaltStringLength = SqlClientDriver.MaxSizeForLengthLimitedString + 1;
            this.DefaltStringIdLength = 128;

            // Set mapper to ignore abstract classes.
            this.IsRootEntity((type, wasDeclared) => type.IsAbstract == false);

            // Set mapper to use native generator for int Ids, Generators.GuidComb generator for Guid Ids and string length of 128 for string Ids.
            this.BeforeMapClass += this.OnMapperOnBeforeMapClass;

            // Set mapper to set a fields size to the properties StringLength attribute if it has one, and non-nullable types to be not nullable in the database.
            this.BeforeMapProperty += this.OnMapperOnBeforeMapProperty;
        }

        /// <summary>
        /// Gets the foreign key field name to use for a property.
        /// </summary>
        private string GetKeyColumnName(IModelInspector inspector, PropertyPath member)
        {
            var otherSideProperty = member.OneToManyOtherSideProperty();
            if (inspector.IsOneToMany(member.LocalMember) && otherSideProperty != null)
            {
                return otherSideProperty.Name + this._foreignKeyColumnSuffix;
            }

            return member.GetRootMemberType().Name + this._foreignKeyColumnSuffix;
        }

        /// <summary>
        /// Gets the Many to Many link tables name.
        /// </summary>
        private string GetManyToManyLinkTableName(PropertyPath member)
        {
            return String.Join(this._manyToManyLinkTableInsert, GetManyToManySidesNames(member).OrderBy(x => x));
        }

        /// <summary>
        /// Sets the mapper to use:
        ///  1) a native generator for int primary keys
        ///  2) a Generators.GuidComb generator for Guid primary keys
        ///  3) a string length of 128 for string primary keys
        /// </summary>
        private void OnMapperOnBeforeMapClass(IModelInspector inspector, Type type, IClassAttributesMapper customizer)
        {
            foreach (var p in type.GetProperties())
            {
                if (inspector.IsPersistentId(p))
                {
                    var idType = p.PropertyType;
                    if (idType == typeof(int))
                    {
                        customizer.Id(x => x.Generator(Generators.Native));
                    }
                    else if (idType == typeof(string))
                    {
                        var customAttributes = p.GetCustomAttributes(false);
                        StringLengthAttribute stringlengthAttribute =
                            (StringLengthAttribute)
                            customAttributes.FirstOrDefault(x => x.GetType() == typeof(StringLengthAttribute));
                        int length = this.DefaltStringIdLength;
                        if (stringlengthAttribute != null && stringlengthAttribute.MaximumLength > 0)
                        {
                            length = stringlengthAttribute.MaximumLength;
                        }
                        customizer.Id(x => x.Length(length));
                    }
                    else if (idType == typeof(Guid))
                    {
                        customizer.Id(x => { x.Generator(Generators.GuidComb); });
                    }
                }
            }
        }

        /// <summary>
        /// Sets the mapper to use:
        ///  1) a properties StringLength attribute if it has one for the databases field size.
        ///  2) non-nullable types to be not nullable in the database.
        ///  3) creates indexes based on the index attributes of properties.
        /// </summary>
        private void OnMapperOnBeforeMapProperty(
            IModelInspector inspector,
            PropertyPath member,
            IPropertyMapper customizer)
        {
            // Get all the custom attributes.
            var customAttributes = member.LocalMember.GetCustomAttributes(false);

            // For all types check for index attributes and add indexes if required.
            var indexAttributes = customAttributes.OfType<IndexAttribute>();
            foreach (var indexAttribute in indexAttributes)
            {
                string indexPrefix = member.GetContainerEntity(inspector).Name;
                if (indexAttribute.Unique)
                {
                    string indexName = $"UI_{indexPrefix}_{indexAttribute.Name}";
                    customizer.UniqueKey(indexName);
                }
                else
                {
                    string indexName = $"IX_{indexPrefix}_{indexAttribute.Name}";
                    customizer.Index(indexName);
                }
            }

            // For string types check for string length attribute and set field length if required
            Type memberType = member.LocalMember.GetPropertyOrFieldType();
            if (memberType == typeof(string))
            {
                StringLengthAttribute stringlengthAttribute =
                    (StringLengthAttribute)
                    customAttributes.FirstOrDefault(x => x.GetType() == typeof(StringLengthAttribute));
                int length = this.DefaltStringLength;
                if (stringlengthAttribute != null && stringlengthAttribute.MaximumLength > 0)
                {
                    length = stringlengthAttribute.MaximumLength;
                }
                customizer.Length(length);
            }

            // For all types if the type is not nullable then set not nullable to true.
            if (!IsNullable(memberType))
            {
                customizer.NotNullable(true);
            }
        }

        /// <summary>
        /// Applies the <see cref="DateTimeOffsetSplitType"/> Composite user type to all <see cref="DateTimeOffset"/> fields in the mapping.
        /// </summary>
        /// <remarks>
        /// Allows the use of <see cref="DateTimeOffset"/> type with databases that do not natively support it.
        /// User: mapper.BeforeMapProperty += ModelMapperHelper.ApplyDateTimeOffsetSplitTypeToDateTimeOffset
        /// </remarks>
        private void ApplyDateTimeOffsetSplitTypeToDateTimeOffset(IModelInspector inspector, PropertyPath property, IPropertyMapper mapper)
        {
            Type propertyType = property.LocalMember.GetPropertyOrFieldType();
            if (propertyType == typeof(DateTimeOffset) || propertyType == typeof(DateTimeOffset?))
            {
                mapper.Type(typeof(DateTimeOffsetSplitType), null);
                string columName = property.ToColumnName();
                mapper.Columns(n => n.Name(columName + "DateTime"), n => n.Name(columName + "Offset"));
            }
        }
    }
}