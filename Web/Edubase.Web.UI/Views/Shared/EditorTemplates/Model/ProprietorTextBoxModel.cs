using System;
using System.Linq.Expressions;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Views.Shared.EditorTemplates.Model;

public class ProprietorTextBoxModel
{
    public Expression<Func<ProprietorViewModel, string>> FieldExpression { get; set; }
    public string FieldName { get; set; }
    public string FieldClasses { get; set; }
}

