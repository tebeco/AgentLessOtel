using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MyWebApp;

[ApiController]
public class RootController(SelfHttpClient selfHttpClient) : ControllerBase
{
    [HttpGet("/")]
    public async Task<ContentHttpResult> Get()
    {
        var livenessResult = await selfHttpClient.GetLivenesstAsync();
        var readinessResult = await selfHttpClient.GetReadinesstAsync();

        return TypedResults.Text($"""
        <ul>
            <li>livenessResult: {livenessResult}</li>
            <li>readinessResult: {readinessResult}</li>
        </ul>
        """, MediaTypeNames.Text.Html);
    }

    [HttpGet("/obsolete")]
    [Obsolete("This endpoint is obsolete. Use '/' instead.")]
    public async Task<ContentHttpResult> GetObsolete()
    {
        var livenessResult = await selfHttpClient.GetLivenesstAsync();
        var readinessResult = await selfHttpClient.GetReadinesstAsync();

        return TypedResults.Text($"""
        <ul>
            <li>livenessResult: {livenessResult}</li>
            <li>readinessResult: {readinessResult}</li>
        </ul>
        """, MediaTypeNames.Text.Html);
    }
}