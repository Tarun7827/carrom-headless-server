using UnityEngine;
namespace Carrom.Headless.Infrastructure
{
    public static class Json { public static string Serialize<T>(T value)=>JsonUtility.ToJson(value); public static T Deserialize<T>(string value)=>JsonUtility.FromJson<T>(value); }
}
