using System;
using System.Collections.Generic;

public interface IJSONObject
{
    IEnumerable<string> GetAllFields();
    string GetField(string fieldName);
    void SetField(string fieldName, string value);
}