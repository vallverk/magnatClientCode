using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //GameDataManager gdata = new GameDataManager();
            //GameField gf = new GameField();
            FieldData d = new FieldData();
            d.FieldGO = new GameObject();
            Assert.AreEqual(d.FieldGO, null);
        }
    }
}
