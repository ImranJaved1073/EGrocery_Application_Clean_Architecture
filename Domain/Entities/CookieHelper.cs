﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public static class CookieHelper
{
    public static void SetCookie(HttpContext context, string key, object value, int? expireTime, string userId)
    {
        CookieOptions option = new CookieOptions();

        if (expireTime.HasValue)
            option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
        else
            option.Expires = DateTime.Now.AddMilliseconds(10);

        var jsonValue = JsonConvert.SerializeObject(value);
        var uniqueKey = $"{key}_{userId}"; // Append userId to make the key unique per user
        context.Response.Cookies.Append(uniqueKey, jsonValue, option);

        // Update the list of user-specific cookie keys
        var userCookieListKey = $"UserCookies_{userId}";
        var existingCookies = context.Request.Cookies[userCookieListKey];

        List<string> userCookies;
        if (existingCookies != null)
        {
            userCookies = JsonConvert.DeserializeObject<List<string>>(existingCookies) ?? new List<string>();
        }
        else
        {
            userCookies = new List<string>();
        }

        if (!userCookies.Contains(uniqueKey))
        {
            userCookies.Add(uniqueKey);
            context.Response.Cookies.Append(userCookieListKey, JsonConvert.SerializeObject(userCookies), option);
        }
    }

    public static T GetCookie<T>(HttpContext context, string key, string userId)
    {
        var uniqueKey = $"{key}_{userId}";
        var value = context.Request.Cookies[uniqueKey];
        if (value == null)
        {
            return default(T)!;
        }
        return JsonConvert.DeserializeObject<T>(value)!;
    }

    public static void ClearCartCookies(HttpContext context, string userId)
    {
        var userCookieListKey = $"UserCookies_{userId}";
        var existingCookies = context.Request.Cookies[userCookieListKey];
        if (existingCookies != null)
        {
            List<string> userCookies = JsonConvert.DeserializeObject<List<string>>(existingCookies) ?? new List<string>();
            foreach (var cookie in userCookies)
            {
                context.Response.Cookies.Delete(cookie);
            }

            // Clear the user-specific cookie list
            context.Response.Cookies.Delete(userCookieListKey);
        }
    }
}
