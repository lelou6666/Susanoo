﻿//#region

//using System;
//using System.Data;
//using System.Data.SqlClient;
//using System.Diagnostics;
//using NUnit.Framework;

//#endregion

//namespace Susanoo.Tests.Scalar
//{
//    [Category("Scalar")]
//    [TestFixture]
//    public class ScalarTests
//    {
//        private readonly DatabaseManager DatabaseManager = Setup.DatabaseManager;

//        [TestFixtureSetUp]
//        public void SetUp()
//        {
//        }

//        [Test]
//        public void NullableScalarAcceptsNull()
//        {
//            var result = CommandManager.Instance.DefineCommand("SELECT CAST(NULL AS INT)", CommandType.Text)
//                .Realize()
//                .ExecuteScalar<int?>(DatabaseManager);

//            Assert.AreEqual(result, null);
//        }

//        [Test]
//        [ExpectedException(typeof(NullReferenceException))]
//        public void NonNullableScalarThrowsIfNull()
//        {
//            CommandManager.Instance.DefineCommand("SELECT CAST(NULL AS INT)", CommandType.Text)
//                .Realize()
//                .ExecuteScalar<int>(DatabaseManager);

//        }
//    }
//}