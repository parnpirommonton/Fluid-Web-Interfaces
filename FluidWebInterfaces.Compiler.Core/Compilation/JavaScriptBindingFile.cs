namespace FluidWebInterfaces.Compiler.Core.Compilation;

public static class JavaScriptBindingFile
{
    private const char lc = '{';
    private const char rc = '}';
    
    public static string Content (string propertiesSplit, string ifCommandsSplit) =>
$"""
class Property
{lc}
    constructor(id, attribute, valueFactory)
    {lc}
        this.Id = id;
        this.Attribute = attribute
        this.ValueFactory = valueFactory;
    {rc}
{rc}

class IfCommand
{lc}
    constructor(id, condition, ifInnerHtml, elseInnerHtml)
    {lc}
        this.Id = id;
        this.Condition = condition;
        this.IfInnerHtml = ifInnerHtml;
        this.ElseInnerHtml = elseInnerHtml;
    {rc}
{rc}

class SavedState
{lc}
    constructor(id, value)
    {lc}
        this.Id = id;
        this.Value = value;
    {rc}
{rc}

let _properties = [{propertiesSplit}];
let _lastPropertyStates = [];

let _ifCommands = [{ifCommandsSplit}];
let _lastIfCommandStates = [];

function onStateChanged()
{lc}
    // Apply changes to if else statements.
    for (let i = 0; i < _ifCommands.length; i++)
    {lc}
       let ifElement = document.querySelectorAll(`[data-if-id="${lc}_ifCommands[i].Id{rc}"]`);
       
       let lastIfCommandState = null;
       for (let j = 0; j < _lastIfCommandStates.length; j++)
       {lc}
           if (_lastIfCommandStates[j].Id === _ifCommands[i].Id)
           {lc}
               lastIfCommandState = _lastIfCommandStates[j];
               break;
           {rc}
       {rc}
       
       if (lastIfCommandState === null)
       {lc}
           lastIfCommandState = new SavedState(_ifCommands[i].Id, null);
           _lastIfCommandStates.push(lastIfCommandState);
       {rc}

       if (ifElement.length === 0)
       {lc}
           lastIfCommandState.Value = null;
           continue;
       {rc}
       
       let condition = _ifCommands[i].Condition();

       if (condition === lastIfCommandState.Value)
       {lc}
           continue;
       {rc}
       
       if (condition)
       {lc}
           ifElement[0].innerHTML = _ifCommands[i].IfInnerHtml;
       {rc}
       else
       {lc}
           ifElement[0].innerHTML = _ifCommands[i].ElseInnerHtml;
       {rc}

        lastIfCommandState.Value = condition;
    {rc}

    // Apply changes to all bound properties.
    for (let i = 0; i < _properties.length; i++)
    {lc}
        let propertyValue = _properties[i].ValueFactory();
        let dataBoundElementSelector = document.querySelectorAll(`[data-bound-properties]`);
        let dataBoundElements = Array.from(dataBoundElementSelector);
        
        let boundElement = null;

        let lastPropertyState = null;
        for (let j = 0; j < _lastPropertyStates.length; j++)
        {lc}
            if (_lastPropertyStates[j].Id === _properties[i].Id)
            {lc}
                lastPropertyState = _lastPropertyStates[j];
                break;
            {rc}
        {rc}

        if (lastPropertyState === null)
        {lc}
            lastPropertyState = new SavedState(_properties[i].Id, null);
            _lastPropertyStates.push(lastPropertyState);
        {rc}
        
        for (let j = 0; j < dataBoundElements.length; j++)
        {lc}
            let boundProperties = dataBoundElements[j].dataset.boundProperties.split(" ");
            for (let n = 0; n < boundProperties.length; n++)
            {lc}
                if (boundProperties[n] === _properties[i].Id)
                {lc}
                    boundElement = dataBoundElements[j];
                {rc}
            {rc}
        {rc}
        
        if (boundElement === null)
        {lc}
            lastPropertyState.Value = null;
            continue;
        {rc}

        if (propertyValue === lastPropertyState.Value)
        {lc}
            continue;
        {rc}
        
        if (_properties[i].Attribute === "innerHTML")
        {lc}
            boundElement.innerHTML = propertyValue;
        {rc}
        else
        {lc}
            boundElement.setAttribute(_properties[i].Attribute, propertyValue);
        {rc}

        lastPropertyState.Value = propertyValue;
    {rc}
{rc}
""";
}