﻿using System.Reflection;

namespace GXOVnT.Shared.Common;

/// <summary>
/// Base enumeration class taken from https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
/// </summary>
public abstract class Enumeration : IComparable
{
    protected Enumeration(string name, int id)
    {
        Name = name;
        Id = id;
    }

    public string Name { get; private set; }

    public int Id { get; private set; }

    protected Enumeration(int id, string name) => (Id, Name) = (id, name);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

    public static IEnumerable<T> GetAll<T>(Func<T, bool> filter) where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>()
            .Where(filter);
    
    public static T? FromValue<T>(int value) where T: Enumeration
    {
        return GetAll<T>().FirstOrDefault(e => e.Id == value);
    }

    public static T? FromName<T>(string name) where T: Enumeration
    {
        return GetAll<T>().FirstOrDefault(e => e.Name.Equals(name));
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }


    public int CompareTo(object? obj) => Id.CompareTo(((Enumeration)obj!)?.Id ?? -1);
    
}

