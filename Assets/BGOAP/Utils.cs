using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class Utils {


    public static IComparable Min( IComparable a, IComparable b ) {
        if(a.CompareTo( b ) <= 0)
            return a;
        else
            return b;
    }

    public static IComparable Max( IComparable a, IComparable b ) {
        if(a.CompareTo( b ) >= 0)
            return a;
        else
            return b;
    }


}

