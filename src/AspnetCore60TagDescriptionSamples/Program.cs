using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using AspnetCore60TagDescriptionSamples;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// using ApiExplorerSettings
//builder.Services.AddSwaggerGen(options =>
//{
//    options.TagActionsBy(apiDescription =>
//    {
//        if (apiDescription.GroupName != null)
//        {
//            return new[] { apiDescription.GroupName };
//        }

//        if (apiDescription.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
//        {
//            return new[] { controllerActionDescriptor.ControllerName };
//        }

//        throw new InvalidOperationException("ƒGƒ‰[");
//    });
//    options.DocInclusionPredicate((name, api) => true);

//    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
//});


// using XML Comment (summary)
//builder.Services.AddSwaggerGen(options =>
//{
//    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//    var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

//    options.TagActionsBy(apiDescription =>
//    {
//        var controllerActionDescriptor = apiDescription.ActionDescriptor as ControllerActionDescriptor;
//        var controllerName = controllerActionDescriptor?.ControllerName;
//        var controllerType = controllerActionDescriptor?.ControllerTypeInfo.ToString();
//        var member = XDocument.Load(xmlFilePath).Root?.XPathSelectElement($"/doc/members/member[@name=\"T:{controllerType}\"]");
//        return new[] { $"{controllerName} : {member?.XPathSelectElement("summary")?.Value}" };
//    });
//    options.DocInclusionPredicate((name, api) => true);

//    options.IncludeXmlComments(xmlFilePath);
//});

// using DocumentFilter
builder.Services.AddSwaggerGen(options =>
{
    options.DocumentFilter<TagDescriptionsDocumentFilter>();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
