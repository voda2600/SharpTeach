namespace OnlineCompiler.Server.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineCompiler.Server.Data;
using OnlineCompiler.Server.Handlers;
using OnlineCompiler.Server.Models;
using OnlineCompiler.Shared;
using System;
using System.Reflection;
using System.Security.Claims;

[ApiController]
[Route("[controller]")]
public class ExecutionController : ControllerBase
{
    private static Dictionary<string, CodeExecutor> _codeExecutors = new Dictionary<string, CodeExecutor>();

    private readonly ILogger<ExecutionController> _logger;
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExecutionController(ILogger<ExecutionController> logger, 
        ApplicationDbContext db, 
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _db = db;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public enum StructureType
    {
        Dictionary,
        HashSet,
        List,
        Queue,
        Stack,
        SortedList,
        LinkedList
    }

    private ExecutionInfo ProcessCode(string? code, StructureType structureType)
    {
        if (code == null) return null;

        var hints = new List<string>();

        try
        {
            Type genericType = structureType switch
            {
                StructureType.Dictionary => typeof(KeyValuePair<string, string>),
                StructureType.SortedList => typeof(KeyValuePair<string, string>),
                _ => typeof(string)
            };

            var dataType = DynamicClassCreator.CreateClassFromCode(code, structureType.ToString());
            Type constructedType = structureType switch
            {
                StructureType.Dictionary => dataType.MakeGenericType(typeof(string), typeof(string)),
                StructureType.SortedList => dataType.MakeGenericType(typeof(string), typeof(string)),
                _ => dataType.MakeGenericType(genericType),
            };

            if (constructedType == null) throw new Exception("Error");

            var hintMethod = typeof(HintReflectionHelper).GetMethod($"GetReflectionHints{structureType}",
                BindingFlags.Public | BindingFlags.Static);
            hintMethod?.Invoke(null, new object[] { code, constructedType, hints });

            bool checkResult = structureType switch
            {
                StructureType.List => CodeCompileChecker<string>.CheckList(code, "first"),
                StructureType.LinkedList => CodeCompileChecker<string>.CheckLinkedList(code, "first"),
                StructureType.SortedList => CodeCompileChecker<string>.CheckSortedList(code, "a", "b"),
                StructureType.Stack => CodeCompileChecker<string>.CheckStack(code, "first"),
                StructureType.HashSet => CodeCompileChecker<string>.CheckHashSet(code, "first"),
                StructureType.Dictionary => CodeCompileChecker<string>.CheckDictionary(code, "key", "value"),
                StructureType.Queue => CodeCompileChecker<string>.CheckQueue(code, "first"),
                _ => false,
            };

            if (checkResult.Equals(false))
                return new ExecutionInfo(
                    ExecutionInfo.ExecutionStatus.WithWarning,
                    0,
                    $"Структура {structureType} не прошла проверку на соответствие с поведением net standard на реальных данных",
                    hints
                );
        }
        catch (TargetInvocationException ex)
        {
            if (ex.InnerException.ToString().Contains("NotImplementedException"))
            {
                return new ExecutionInfo(ExecutionInfo.ExecutionStatus.CompilationError,
                    $"В структуре все еще остались методы, которые требуют реализации. Обратите внимание на комментарии в коде с текстом //Нужна реализация.");
            }

            return new ExecutionInfo(
                ExecutionInfo.ExecutionStatus.CompilationError,
                0,
                $"Произошла ошибка при запуске структуры {structureType}: {ex}",
                hints
            );
        }
        catch (ArgumentException e)
        {
            return new ExecutionInfo(ExecutionInfo.ExecutionStatus.CompilationError, e.Message);
        }
        catch (Exception ex)
        {
            return new ExecutionInfo(
                ExecutionInfo.ExecutionStatus.CompilationError,
                0,
                $"Произошла ошибка при запуске структуры {structureType}: {ex}",
                hints
            );
        }

        return new ExecutionInfo(
            hints.Count > 0 ? ExecutionInfo.ExecutionStatus.WithWarning : ExecutionInfo.ExecutionStatus.Finished, 111,
            "", hints);
    }

    /// <summary>
    /// Request for compiling and executing code
    /// </summary>
    /// <param name="code">C# code</param>
    /// <param name="structureType">type of structure that should be checked</param>
    /// <returns>Unique id of the operation</returns>
    [HttpPost]
    [Route("{structureType}")]
    public ExecutionInfo Post([FromBody] RequestData data, StructureType structureType)
    {
        if (data.Code != null)
        {
            StructureInfo structureInfo = new StructureInfo()
            {
                UserLogin = data.Login,
                StructureType = structureType,
                LastSaved = DateTime.UtcNow,
                Code = data.Code
            };
            _db.StructureInfos.Add(structureInfo);
            _db.SaveChanges();
        }

        return ProcessCode(data.Code, structureType);
    }

    [HttpPost]
    [Route("GetCode/{structureType}")]
    public string Post([FromBody] string username, StructureType structureType)
    {
        var structure = _db.StructureInfos.OrderByDescending(x=>x.LastSaved).FirstOrDefault(s => s.UserLogin == username
                                                          && s.StructureType == structureType);
        return structure == null ? "" : structure.Code;
    }

    /// <summary>
    /// Getting execution information by id
    /// </summary>
    /// <param name="id">id</param>
    /// <returns>Execution Information</returns>
    [HttpGet]
    [Route("{id}")]
    public ExecutionInfo? Get(string id)
    {
        if (!_codeExecutors.ContainsKey(id))
            return null;
        return _codeExecutors[id].ExecutionInfo;
    }
}