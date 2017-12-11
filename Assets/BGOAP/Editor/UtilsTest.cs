using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class UtilsTest {

	[Test]
	public void MinMax() {

        Assert.AreEqual( Utils.Min( 10, 8), 8, "min int" );
        Assert.AreEqual( Utils.Min( 9f, 5f ), 5f, "min float" );

        Assert.AreEqual( Utils.Max( 2, 4 ), 4, "max int" );
        Assert.AreEqual( Utils.Max( 3f, 6f ), 6f, "max float " );

    }



}
