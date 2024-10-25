using System.Linq;
using System.Text;

namespace GrammarGuide.ContentBuilder.Prompts;

public class PromptBuilder
{
    private readonly StringBuilder _data = new();
    private readonly IPromptDataProvider _promptDataProvider;

    public PromptBuilder(IPromptDataProvider promptDataProvider)
    {
        _promptDataProvider = promptDataProvider;
    }

    public PromptBuilder AddPersona(string persona, params string[] args)
    {
        if (!string.IsNullOrEmpty(persona) && ArgsCheck(args))
            _data.Append(string.Format(_promptDataProvider.GetPersona(persona), args));

        return this;
    }
    
    public PromptBuilder AddContext(string context, params string[] args)
    {
        if (!string.IsNullOrEmpty(context) && ArgsCheck(args))
            _data.Append(string.Format(_promptDataProvider.GetContext(context), args));

        return this;
    }
    
    public PromptBuilder AddTask(string task, params string[] args)
    {
        if (!string.IsNullOrEmpty(task) && ArgsCheck(args))
            _data.Append(string.Format(_promptDataProvider.GetTask(task), args));

        return this;
    }
    public PromptBuilder AddTask(string task)
    {
        if (!string.IsNullOrEmpty(task) )
            _data.Append(string.Format(_promptDataProvider.GetTask(task)));

        return this;
    }
    
    public PromptBuilder AddFormat(string format, params string[] args)
    {
        if (!string.IsNullOrEmpty(format) && ArgsCheck(args))
            _data.Append(string.Format(_promptDataProvider.GetFormat(format), args));
        
        return this;
    }

    private bool ArgsCheck(string[] args)
    {
        return args.Any() && args.All(a => !string.IsNullOrEmpty(a));
    }

    public PromptBuilder AddCustomContext(string customContext)
    {
        if (!string.IsNullOrEmpty(customContext))
            _data.Append(customContext);
        return this;
    }

    public string Build()
    {
        return _data.ToString();
    }
}