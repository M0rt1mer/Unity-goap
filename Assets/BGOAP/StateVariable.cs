using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region IWorldState


public interface IStateVarKey {

    IStateVariableLogic Logic { get; }
    string Name { get; }
    object GetDefaultValue();
    float Distance( object initial, object goal );

}

/// <summary>
/// IStateVarKey is an key for for BGoapState. Each Key/Value pair represent a statement about the NPC (e.g. "is at world position X"). 
/// </summary>
/// <typeparam name="ValueType">Type for values, which can be stored under this key. This is guaranteed only in BGoapState</typeparam>
public abstract class AStateVarKey<ValueType> : IStateVarKey {
    public abstract IStateVariableLogic Logic { protected set; get; }
    public abstract string Name { protected set; get; }

    public abstract float Distance( object a, object b );
    public abstract object GetDefaultValue();

    public ValueType GetTypedDefaultValue() {
        return (ValueType)GetDefaultValue();
    }
}

/// Used for WorldStates with simple logic - it doesn't need to be comparable (usually EQUAL logic)
/// </summary>
/// <typeparam name="ValueType"></typeparam>
public class StateVarKey<ValueType> : AStateVarKey<ValueType> {

    public override IStateVariableLogic Logic { protected set; get; }
    public override string Name { protected set; get; }

    private ValueType defaultValue;
    protected Func<object, object, float> distanceFunc;

    public StateVarKey( string name, ValueType defaultValue ){
        this.Logic = StateVariableLogicFactory.GetWorldStateLogic<StateVariableLogicEquals>();
        this.Name = name;
        this.defaultValue = defaultValue;
    }

    public override string ToString() {
        return string.Format( "WorldState[{0}]", typeof(ValueType).Name );
    }

    public override float Distance(object a, object b){
        if (distanceFunc == null)
            return 1;
        return distanceFunc(a, b);
    }

    public override object GetDefaultValue() {
        return defaultValue;
    }
}

/// <summary>
/// Used for more advanced WorldStateLogics, that require IComparable value types
/// </summary>
/// <typeparam name="InnerType"></typeparam>
/// <typeparam name="LogicType"></typeparam>
public class StateVarKeyComparable<InnerType,LogicType> : StateVarKey<InnerType> 
            where InnerType : IComparable 
            where LogicType : StateVariableLogic<IComparable>,new() {

    public StateVarKeyComparable( string name, InnerType defaultValue ) : base( name, defaultValue ){
        this.Logic = StateVariableLogicFactory.GetWorldStateLogic<LogicType>();
    }

    public StateVarKeyComparable(string name, InnerType defaultValue, Func<object,object,float> distanceFnc ) : base(name, defaultValue){
        this.Logic = StateVariableLogicFactory.GetWorldStateLogic<LogicType>();
        this.distanceFunc = distanceFnc;
    }

    public override string ToString() {
        return string.Format( "WorldState{1}[{0}]", typeof(InnerType).Name, Logic.ToString() );
    }

}

/*public interface IGenericStateVarKeyTemplate {
    /// <summary>
    /// TODO: make robust check
    /// </summary>
    /// <param name="argument"></param>
    /// <returns></returns>
    IStateVarKey MakeGenericInstance(object argument);
}*/

public interface IGenericStateVarKeyTemplate<GenericType>// : IGenericStateVarKeyTemplate
{
    IStateVarKey MakeGenericInstance(GenericType argument);
}

public class GenericStateVarKeyTemplate<ValueType, GenericType> : StateVarKey<ValueType>, IGenericStateVarKeyTemplate<GenericType>
{
    public GenericStateVarKeyTemplate(string name, ValueType defaultValue) : base(name, defaultValue)
    {
    }

    public IStateVarKey MakeGenericInstance(object argument)
    {
        throw new NotImplementedException();
    }

    public GenericStateVarKey<ValueType, GenericType> MakeGenericInstance( GenericType argument ) {
        return new GenericStateVarKey<ValueType, GenericType>(this, argument);
    }

    IStateVarKey IGenericStateVarKeyTemplate<GenericType>.MakeGenericInstance(GenericType argument)
    {
        return MakeGenericInstance((GenericType)argument);
    }
}

/*public interface IGenericStateVarKey {
    IGenericStateVarKeyTemplate GetParent();
}*/

public class GenericStateVarKey<ValueType, GenericArgument> : StateVarKey<ValueType>//, IGenericStateVarKey
{
    GenericStateVarKeyTemplate<ValueType, GenericArgument> parent;
    GenericArgument argument;

    public GenericStateVarKey(GenericStateVarKeyTemplate<ValueType, GenericArgument> parent, GenericArgument argument) :
                    base( parent.Name + string.Format("<{0}>", "tmp" /*argument*/ ), parent.GetTypedDefaultValue() ) {
        this.parent = parent;
        this.argument = argument;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (!(obj is GenericStateVarKey<ValueType, GenericArgument>))
            return false;
        GenericStateVarKey<ValueType, GenericArgument> other = obj as GenericStateVarKey<ValueType, GenericArgument>;
        return other.parent == parent && other.argument.Equals(argument);
    }

    public override int GetHashCode()
    {
        return parent.GetHashCode()^argument.GetHashCode();
    }

   /* public IGenericStateVarKeyTemplate GetParent()
    {
        return parent;
    }*/
}

#endregion
