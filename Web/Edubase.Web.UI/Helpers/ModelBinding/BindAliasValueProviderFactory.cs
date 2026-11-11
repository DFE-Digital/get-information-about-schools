using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ModelBinding
{
    internal sealed class BindAliasValueProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var aliasMap = DefaultModelBinderEx.BuildAliasMap(context.ActionContext);
            if (aliasMap.Count == 0)
            {
                return Task.CompletedTask;
            }

            context.ValueProviders.Insert(0, new BindAliasValueProvider(aliasMap, context.ActionContext.HttpContext.Request.Query));
            return Task.CompletedTask;
        }

        private sealed class BindAliasValueProvider : IValueProvider
        {
            private readonly IReadOnlyDictionary<string, string[]> _aliasMap;
            private readonly IQueryCollection _query;

            public BindAliasValueProvider(IReadOnlyDictionary<string, string[]> aliasMap, IQueryCollection query)
            {
                _aliasMap = aliasMap;
                _query = query;
            }

            public bool ContainsPrefix(string prefix)
            {
                if (string.IsNullOrWhiteSpace(prefix))
                {
                    return false;
                }

                foreach (var candidate in ExpandAliasCandidates(prefix))
                {
                    if (_query.ContainsKey(candidate))
                    {
                        return true;
                    }
                }

                return false;
            }

            public ValueProviderResult GetValue(string key)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    return ValueProviderResult.None;
                }

                foreach (var candidate in ExpandAliasCandidates(key))
                {
                    if (_query.TryGetValue(candidate, out var values))
                    {
                        return new ValueProviderResult(values, CultureInfo.CurrentCulture);
                    }
                }

                return ValueProviderResult.None;
            }

            private IEnumerable<string> ExpandAliasCandidates(string key)
            {
                var emitted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                yield return key;
                emitted.Add(key);

                var lastDot = key.LastIndexOf('.');
                if (lastDot >= 0)
                {
                    var prefix = key.Substring(0, lastDot + 1);
                    var property = key.Substring(lastDot + 1);

                    if (_aliasMap.TryGetValue(property, out var aliases))
                    {
                        foreach (var alias in aliases)
                        {
                            var candidate = prefix + alias;
                            if (emitted.Add(candidate))
                            {
                                yield return candidate;
                            }
                        }
                    }

                    yield break;
                }

                if (!_aliasMap.TryGetValue(key, out var topLevelAliases))
                {
                    yield break;
                }

                foreach (var alias in topLevelAliases)
                {
                    if (emitted.Add(alias))
                    {
                        yield return alias;
                    }
                }
            }
        }
    }
}
