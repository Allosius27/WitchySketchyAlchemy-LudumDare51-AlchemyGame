//
// Updated by Allosius(Yanis Q.) on 7/9/2022.
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllosiusDevUtilities
{
    [AttributeUsage(AttributeTargets.Field)]

    public class BaseCustomAttribute : Attribute { }

    public class IDAttribute : BaseCustomAttribute { }

    public class TaskIDAttribute : IDAttribute { }

    public class AllosiusDevDataListAttribute : BaseCustomAttribute { }
}
