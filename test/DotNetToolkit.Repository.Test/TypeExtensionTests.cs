namespace DotNetToolkit.Repository.Test
{
    using Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Utility;
    using Xunit;

    public class TypeExtensionTests
    {
        [Fact]
        public void InvokeConstructorWithParameters()
        {
            TestInvokeConstructor<TestClassOne, string>(x => x.PString, "hello world", "hello world", expToParameter: true);
            TestInvokeConstructor<TestClassOne, bool>(x => x.PBool, "true", true, expToParameter: true);
            TestInvokeConstructor<TestClassOne, bool>(x => x.PBool, "false", false, expToParameter: true);
            TestInvokeConstructor<TestClassOne, bool>(x => x.PBool, "1", true, expToParameter: true);
            TestInvokeConstructor<TestClassOne, bool>(x => x.PBool, "0", false, expToParameter: true);
            TestInvokeConstructor<TestClassOne, bool>(x => x.PBool, "on", true, expToParameter: true);
            TestInvokeConstructor<TestClassOne, bool>(x => x.PBool, "off", false, expToParameter: true);
            TestInvokeConstructor<TestClassOne, TimeSpan>(x => x.PTimeSpan, "00:00:30", TimeSpan.FromSeconds(30), expToParameter: true);
            TestInvokeConstructor<TestClassOne, DateTime>(x => x.PDateTime, "2005-05-05", DateTime.Parse("2005-05-05"), expToParameter: true);
            TestInvokeConstructor<TestClassOne, int>(x => x.PInt, "1", 1, expToParameter: true);
            TestInvokeConstructor<TestClassOne, double>(x => x.PDouble, "1", 1.00, expToParameter: true);
            TestInvokeConstructor<TestClassOne, decimal>(x => x.PDecimal, "1.00", 1, expToParameter: true);
            TestInvokeConstructor<TestClassOne, MyEnum>(x => x.PMyEnum, "One", MyEnum.One, expToParameter: true);
        }

        [Fact]
        public void InvokeConstructorWithNullableParameters()
        {
            TestInvokeConstructor<TestClassOne, string>(x => x.PString, "", string.Empty, expToParameter: true);
            TestInvokeConstructor<TestClassOne, bool?>(x => x.PNullableBool, "", (bool?)null, expToParameter: true);
            TestInvokeConstructor<TestClassOne, bool?>(x => x.PNullableBool, "", (bool?)null, expToParameter: true);
            TestInvokeConstructor<TestClassOne, bool?>(x => x.PNullableBool, "", (bool?)null, expToParameter: true);
            TestInvokeConstructor<TestClassOne, bool?>(x => x.PNullableBool, "", (bool?)null, expToParameter: true);
            TestInvokeConstructor<TestClassOne, bool?>(x => x.PNullableBool, "", (bool?)null, expToParameter: true);
            TestInvokeConstructor<TestClassOne, bool?>(x => x.PNullableBool, "", (bool?)null, expToParameter: true);
            TestInvokeConstructor<TestClassOne, TimeSpan?>(x => x.PNullableTimeSpan, "", (TimeSpan?)null, expToParameter: true);
            TestInvokeConstructor<TestClassOne, DateTime?>(x => x.PNullableDateTime, "", (DateTime?)null, expToParameter: true);
            TestInvokeConstructor<TestClassOne, int?>(x => x.PNullableInt, "", (int?)null, expToParameter: true);
            TestInvokeConstructor<TestClassOne, double?>(x => x.PNullableDouble, "", (double?)null, expToParameter: true);
            TestInvokeConstructor<TestClassOne, decimal?>(x => x.PNullableDecimal, "", (decimal?)null, expToParameter: true);
            TestInvokeConstructor<TestClassOne, MyEnum?>(x => x.PNullableMyEnum, "", (MyEnum?)null, expToParameter: true);
        }

        [Fact]
        public void InvokeConstructorCannotAssignReadOnlyProperties()
        {
            TestInvokeConstructor<TestClassThree, string>(x => x.PStringReadOnlyOne, "hello world", null, expToParameter: false);
            TestInvokeConstructor<TestClassThree, string>(x => x.PStringReadOnlyTwo, "hello world", null, expToParameter: false);
            TestInvokeConstructor<TestClassThree, string>(x => x.PStringReadOnlyThree, "hello world", null, expToParameter: false);
        }

        [Fact]
        public void InvokeConstructorWithProperties()
        {
            TestInvokeConstructor<TestClassTwo, string>(x => x.PString, "hello world", "hello world", expToParameter: false);
            TestInvokeConstructor<TestClassTwo, bool>(x => x.PBool, "true", true, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, bool>(x => x.PBool, "false", false, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, bool>(x => x.PBool, "1", true, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, bool>(x => x.PBool, "0", false, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, bool>(x => x.PBool, "on", true, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, bool>(x => x.PBool, "off", false, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, TimeSpan>(x => x.PTimeSpan, "00:00:30", TimeSpan.FromSeconds(30), expToParameter: false);
            TestInvokeConstructor<TestClassTwo, DateTime>(x => x.PDateTime, "2005-05-05", DateTime.Parse("2005-05-05"), expToParameter: false);
            TestInvokeConstructor<TestClassTwo, int>(x => x.PInt, "1", 1, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, double>(x => x.PDouble, "1", 1.00, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, decimal>(x => x.PDecimal, "1.00", 1, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, MyEnum>(x => x.PMyEnum, "One", MyEnum.One, expToParameter: false);
        }

        [Fact]
        public void InvokeConstructorWithNullableProperties()
        {
            TestInvokeConstructor<TestClassTwo, string>(x => x.PString, "", string.Empty, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, bool?>(x => x.PNullableBool, "", (bool?)null, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, bool?>(x => x.PNullableBool, "", (bool?)null, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, bool?>(x => x.PNullableBool, "", (bool?)null, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, bool?>(x => x.PNullableBool, "", (bool?)null, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, bool?>(x => x.PNullableBool, "", (bool?)null, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, bool?>(x => x.PNullableBool, "", (bool?)null, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, TimeSpan?>(x => x.PNullableTimeSpan, "", (TimeSpan?)null, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, DateTime?>(x => x.PNullableDateTime, "", (DateTime?)null, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, int?>(x => x.PNullableInt, "", (int?)null, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, double?>(x => x.PNullableDouble, "", (double?)null, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, decimal?>(x => x.PNullableDecimal, "", (decimal?)null, expToParameter: false);
            TestInvokeConstructor<TestClassTwo, MyEnum?>(x => x.PNullableMyEnum, "", (MyEnum?)null, expToParameter: false);
        }

        [Fact]
        public void InvokeConstructorWithPropertiesAndParameters()
        {
            var parameters = new Dictionary<string, string>();

            parameters.Add("pString", "hello world");
            parameters.Add("pBool", "true");
            parameters.Add("PDateTime", "2005-05-05");

            var obj = (TestClassFour)typeof(TestClassFour).InvokeConstructor(parameters);

            Assert.Equal("hello world", obj.PString);
            Assert.Equal(true, obj.PBool);
            Assert.Equal(DateTime.Parse("2005-05-05"), obj.PDateTime);
        }

        [Fact]
        public void InvokeConstructorSetsParametersToDefaultWithPartial()
        {
            var parameters = new Dictionary<string, string>();

            parameters.Add("pStringOne", "hello world");

            var obj = (TestClassFive)typeof(TestClassFive).InvokeConstructor(parameters);

            Assert.Equal("hello world", obj.PStringOne);
            Assert.Equal(null, obj.PStringTwo);
        }

        private static void TestInvokeConstructor<T, TExpectedParameterType>(Expression<Func<T, TExpectedParameterType>> exp, string paramValue, TExpectedParameterType expectedValue, bool expToParameter)
        {
            var paramName = ExpressionHelper.GetPropertyName(exp);

            // first letter to lower by convention (assumes that the property expression matches the constructor parameter)
            if (expToParameter)
            {
                paramName = char.ToLower(paramName[0]) + paramName.Substring(1);
            }

            var type = typeof(T);
            var parameters = new Dictionary<string, string> { { paramName, paramValue } };
            var obj = (T)type.InvokeConstructor(parameters);

            var pi = ExpressionHelper.GetPropertyInfo(exp);

            var actualValue = pi.GetValue(obj);

            Assert.Equal(expectedValue, actualValue);
        }

        private class TestClassOne
        {
            public string PString { get; }
            public bool PBool { get; }
            public TimeSpan PTimeSpan { get; }
            public DateTime PDateTime { get; }
            public int PInt { get; }
            public decimal PDecimal { get; }
            public double PDouble { get; }
            public byte PByte { get; }
            public MyEnum PMyEnum { get; }
            public bool? PNullableBool { get; }
            public TimeSpan? PNullableTimeSpan { get; }
            public DateTime? PNullableDateTime { get; }
            public int? PNullableInt { get; }
            public decimal? PNullableDecimal { get; }
            public double? PNullableDouble { get; }
            public byte? PNullableByte { get; }
            public MyEnum? PNullableMyEnum { get; }

            public TestClassOne() { }
            public TestClassOne(string pString) { PString = pString; }
            public TestClassOne(bool pBool) { PBool = pBool; }
            public TestClassOne(TimeSpan pTimeSpan) { PTimeSpan = pTimeSpan; }
            public TestClassOne(DateTime pDateTime) { PDateTime = pDateTime; }
            public TestClassOne(int pInt) { PInt = pInt; }
            public TestClassOne(byte pByte) { PByte = pByte; }
            public TestClassOne(decimal pDecimal) { PDecimal = pDecimal; }
            public TestClassOne(double pDouble) { PDouble = pDouble; }
            public TestClassOne(MyEnum pMyEnum) { PMyEnum = pMyEnum; }
            public TestClassOne(bool? pNullableBool) { PNullableBool = pNullableBool; }
            public TestClassOne(TimeSpan? pNullableTimeSpan) { PNullableTimeSpan = pNullableTimeSpan; }
            public TestClassOne(DateTime? pNullableDateTime) { PNullableDateTime = pNullableDateTime; }
            public TestClassOne(int? pNullableInt) { PNullableInt = pNullableInt; }
            public TestClassOne(byte? pNullableByte) { PNullableByte = pNullableByte; }
            public TestClassOne(decimal? pNullableDecimal) { PNullableDecimal = pNullableDecimal; }
            public TestClassOne(double? pNullableDouble) { PNullableDouble = pNullableDouble; }
            public TestClassOne(MyEnum? pNullableMyEnum) { PNullableMyEnum = pNullableMyEnum; }
        }

        private class TestClassTwo
        {
            public string PString { get; set; }
            public bool PBool { get; set; }
            public TimeSpan PTimeSpan { get; set; }
            public DateTime PDateTime { get; set; }
            public int PInt { get; set; }
            public decimal PDecimal { get; set; }
            public double PDouble { get; set; }
            public byte PByte { get; set; }
            public MyEnum PMyEnum { get; set; }
            public bool? PNullableBool { get; set; } = true;
            public TimeSpan? PNullableTimeSpan { get; set; } = TimeSpan.Zero;
            public DateTime? PNullableDateTime { get; set; } = DateTime.Today;
            public int? PNullableInt { get; set; } = 0;
            public decimal? PNullableDecimal { get; set; } = 0;
            public double? PNullableDouble { get; set; } = 0;
            public byte? PNullableByte { get; set; } = new byte();
            public MyEnum? PNullableMyEnum { get; set; } = MyEnum.One;
        }

        private class TestClassThree
        {
            public string PStringReadOnlyOne { get; }
            public string PStringReadOnlyTwo { get; internal set; }
            public string PStringReadOnlyThree { get; private set; }
        }

        private class TestClassFour
        {
            public string PString { get; }
            public bool PBool { get; }
            public DateTime PDateTime { get; set; }

            public TestClassFour(string pString, bool pBool) { PString = pString; PBool = pBool; }
        }

        private class TestClassFive
        {
            public string PStringOne { get; }
            public string PStringTwo { get; }

            public TestClassFive(string pStringOne, string pStringTwo)
            {
                PStringOne = pStringOne;
                PStringTwo = pStringTwo;
            }
        }

        private enum MyEnum
        {
            One,
            Two
        }
    }
}
