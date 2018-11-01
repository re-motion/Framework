use RpaTest

alter table TableWithAllDataTypes nocheck constraint FK_TableWithAllDataTypes_TableForRelationTestMandatory
alter table TableWithAllDataTypes nocheck constraint FK_TableWithAllDataTypes_TableForRelationTestOptional

delete from [TableForRelationTest]
delete from [TableWithAllDataTypes]
delete from [TableWithUndefinedEnum]

-- TableWithAllDataTypes

-- Note: No test data is loaded for image columns.
insert into [TableWithAllDataTypes] (ID, ClassID, [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [ExtensibleEnum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [StringWithoutMaxLength], [DelimitedStringArray], [Binary],
    [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaEnum], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], 
    [StringWithNullValue], [ExtensibleEnumWithNullValue], [DelimitedNullStringArray], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaEnumWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary],
    [TableForRelationTestMandatory], [TableForRelationTestOptional]) 
    values 
    ('{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}', 'ClassWithAllDataTypes', 
    0, 85, '2005/01/01', '2005/01/01 17:00', 123456.789, 987654.321, 1, 'Remotion.Data.DomainObjects.Web.Test.Domain.ColorExtensions.Green', '{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}', 32767, 2147483647, 9223372036854775807, 6789.321, 'abcdeföäü', '12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890', 'Arthur;Dent;42', 0x12345678901234567890,
    1, 78, '2005/02/01', '2005/02/01 05:00', 765.098, 654321.789, 2, '{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}', 12000, -2147483647, 3147483647, 12.456,
    null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
    '{0498C3E5-30FA-45f1-A9A1-966D4655C3C7}', '{EDCFDFDB-C7B8-4f9b-BC75-72DDB8E19C42}')
    
insert into [TableWithAllDataTypes] (ID, ClassID, [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [ExtensibleEnum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [StringWithoutMaxLength], [DelimitedStringArray], [Binary],
    [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaEnum], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], 
    [StringWithNullValue], [ExtensibleEnumWithNullValue], [DelimitedNullStringArray], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaEnumWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary],
    [TableForRelationTestMandatory], [TableForRelationTestOptional]) 
    values 
    ('{583EC716-8443-4b55-92BF-09F7C8768529}', 'ClassWithAllDataTypes', 
    1, 86, '2005/01/02', '2005/01/02 01:00', 654321.987, 456789.123, 0, 'Remotion.Data.DomainObjects.Web.Test.Domain.LightColorExtensions.LightRed','{D2146236-FBD4-4b93-A835-26563FE3F043}', -32767, -2147483647, -9223372036854775807, -6789.321, 'üäöfedcba', '09876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321', 'Zaphod;Beeblebrox;Heart of Gold;Trillian;Marvin;Deep Thought', 0x12345678901234567890,
    1, 79, '2005/02/02', '2005/02/02 15:00', 123.111, -654321.789, 2, '{8C21745D-1269-4cb2-BC81-E1B112A0C146}', 390, 2147483647, 2147483648, 321.321,
    null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
    '{7E86BD49-6D7D-4d73-87AC-9F7C5F46DEFB}', null)
    
insert into [TableWithAllDataTypes] (ID, ClassID, [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [ExtensibleEnum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [StringWithoutMaxLength], [DelimitedStringArray], [Binary],
    [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaEnum], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], 
    [StringWithNullValue], [ExtensibleEnumWithNullValue], [DelimitedNullStringArray], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaEnumWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary],
    [TableForRelationTestMandatory], [TableForRelationTestOptional]) 
    values 
    ('{D2146236-FBD4-4b93-A835-26563FE3F043}', 'ClassWithAllDataTypes', 
    0, 85, '2005/01/01', '2005/01/01 17:00', 123456.789, 987654.321, 1, 'Remotion.Data.DomainObjects.Web.Test.Domain.MetallicColorExtensions.BlueMetallic', '{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}', 32767, 2147483647, 9223372036854775807, 6789.321, 'abcdeföäü', '12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890', 'Arthur;Dent;42', 0x12345678901234567890,
    1, 78, '2005/02/01', '2005/02/01 05:00', 765.098, 654321.789, 2, '{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}', 12000, -2147483647, 3147483647, 12.456,
    null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
    '{0498C3E5-30FA-45f1-A9A1-966D4655C3C7}', '{EDCFDFDB-C7B8-4f9b-BC75-72DDB8E19C42}')

-- TableForRelationTest
insert into [TableForRelationTest] (ID, ClassID, [Name], [TableWithAllDataTypesMandatory], [TableWithAllDataTypesOptional])
    values ('{ED85682D-CD85-4af7-972B-FAFCBD3C7699}', 'ClassForRelationTest', 'RelationTest1', '{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}', null)
insert into [TableForRelationTest] (ID, ClassID, [Name], [TableWithAllDataTypesMandatory], [TableWithAllDataTypesOptional])
    values ('{F1EAB10F-D3B4-4eb2-8F23-51CE855DB3CC}', 'ClassForRelationTest', 'RelationTest2', '{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}', '{D2146236-FBD4-4b93-A835-26563FE3F043}')
insert into [TableForRelationTest] (ID, ClassID, [Name], [TableWithAllDataTypesMandatory], [TableWithAllDataTypesOptional])
    values ('{EDCFDFDB-C7B8-4f9b-BC75-72DDB8E19C42}', 'ClassForRelationTest', 'RelationTest3', '{D2146236-FBD4-4b93-A835-26563FE3F043}', '{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}')    
insert into [TableForRelationTest] (ID, ClassID, [Name], [TableWithAllDataTypesMandatory], [TableWithAllDataTypesOptional])
    values ('{0498C3E5-30FA-45f1-A9A1-966D4655C3C7}', 'ClassForRelationTest', 'RelationTest4', '{D2146236-FBD4-4b93-A835-26563FE3F043}', null)
insert into [TableForRelationTest] (ID, ClassID, [Name], [TableWithAllDataTypesMandatory], [TableWithAllDataTypesOptional])
    values ('{7E86BD49-6D7D-4d73-87AC-9F7C5F46DEFB}', 'ClassForRelationTest', 'RelationTest5', '{583EC716-8443-4b55-92BF-09F7C8768529}', null)            
insert into [TableForRelationTest] (ID, ClassID, [Name], [TableWithAllDataTypesMandatory], [TableWithAllDataTypesOptional])
    values ('{9E86BD49-6D7D-4d73-87AC-9F7C5F46DEFB}', 'ClassForRelationTest', 'RelationTest6', '{583EC716-8443-4b55-92BF-09F7C8768529}', null)            
    
-- TableWithUndefinedEnum
insert into [TableWithUndefinedEnum] (ID, ClassID, [UndefinedEnum])
    values ('{4F85CEE5-A53A-4bc5-B9D3-448C48946498}', 'ClassWithUndefinedEnum', 1)            
    
alter table TableWithAllDataTypes check constraint FK_TableWithAllDataTypes_TableForRelationTestMandatory
alter table TableWithAllDataTypes check constraint FK_TableWithAllDataTypes_TableForRelationTestOptional
