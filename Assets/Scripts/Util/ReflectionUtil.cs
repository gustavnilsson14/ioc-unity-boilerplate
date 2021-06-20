using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReflectionUtil
{
    public static bool CopyObjectValues(object original, object target) {
        if (!GetStoredObject(out StoredObject storedObject, original))
            return false;
        if (!ApplyStoredObject(storedObject, target))
            return false;
        return true;
    }
    public static bool GetStoredObject(out StoredObject storedObject, object original, Dictionary<string, string> id = null) {
        storedObject = null;
        if (original == null)
            return false;
        storedObject = new StoredObject(original, id);
        return true;
    }
    public static bool ApplyStoredObject(StoredObject storedObject, object target) {
        if (storedObject.objectType != target.GetType()) 
            return false;
        storedObject.ApplyTo(target);
        return true;
    }
    public static bool IsSubClassOrClass<T>(Type t) {
        if (t.IsSubclassOf(typeof(T)) || t == typeof(T))
            return true;
        return false;
    }

    public static Dictionary<string,string> StoredObjectToDict(StoredObject storedObject)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        storedObject.values.ForEach(x => result.Add(x.fieldName,x.valueString));
        return result;
    }

    public static List<Type> GetAllImplementationsOfInterface<T>()
    {
        return Assembly.GetExecutingAssembly().GetTypes().Where(myType => myType.GetInterfaces().Contains(typeof(T))).ToList();
    }
}
[System.Serializable]
public class StoredObject
{
    public Dictionary<string, string> id;
    public Type objectType;
    public string objectTypeName;
    public List<StoredObjectValue> values;
    private BindingFlags flags;
    public StoredObject(object original, Dictionary<string, string> id, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default) {
        this.objectType = original.GetType();
        this.objectTypeName = original.GetType().ToString();
        this.id = id;
        this.flags = flags;
        values = new List<StoredObjectValue>();
        foreach (FieldInfo fieldInfo in objectType.GetFields(flags))
        {
            object value = fieldInfo.GetValue(original);
            if (!IsValidValueForCopy(value))
                continue;
            values.Add(new StoredObjectValue(fieldInfo.Name, value));
        }
    }
    public void ApplyTo(object target)
    {
        foreach (StoredObjectValue storedObjectValue in values)
        {
            FieldInfo fieldInfo = objectType.GetField(storedObjectValue.fieldName);
            if (fieldInfo == null)
                continue;
            fieldInfo.SetValue(target, storedObjectValue.value);
        }
    }
    public bool IsValidValueForCopy(object value)
    {
        if (value is string)
            return true;
        if (value is bool)
            return true;
        if (value is float)
            return true;
        if (value is int)
            return true;
        if (value is StoredObject)
            return true;
        if (value is List<StoredObject>)
            return true;
        if (value is Enum)
            return true;
        return false;
    }
}
[System.Serializable]
public class StoredObjectValue
{
    public string fieldName;
    public object value;
    public string valueString;
    public StoredObjectValue(string fieldName, object value)
    {
        this.fieldName = fieldName;
        this.value = value;
        this.valueString = value.ToString();
    }
}