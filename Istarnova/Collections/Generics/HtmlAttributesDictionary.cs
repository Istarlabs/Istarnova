#region "Header"

// File: HtmlAttributesDictionary.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Buffers;
using System.Text;
using Istarnova.Extensions;

#endregion

namespace Istarnova.Collections.Generics;

public class HtmlAttributesDictionary : Dictionary<string, object>
{
    #region HTML Attribute Constants

    // Core attributes
    public const string Id = "id";
    public const string Class = "class";
    public const string Style = "style";
    public const string Title = "title";
    public const string Lang = "lang";
    public const string Dir = "dir";
    public const string AccessKey = "accesskey";
    public const string TabIndex = "tabindex";
    public const string ContentEditable = "contenteditable";
    public const string Draggable = "draggable";
    public const string Hidden = "hidden";
    public const string SpellCheck = "spellcheck";

    // Form attributes
    public const string Name = "name";
    public const string Value = "value";
    public const string Type = "type";
    public const string Placeholder = "placeholder";
    public const string Required = "required";
    public const string Disabled = "disabled";
    public const string Readonly = "readonly";
    public const string Checked = "checked";
    public const string Selected = "selected";
    public const string Multiple = "multiple";
    public const string AutoComplete = "autocomplete";
    public const string AutoFocus = "autofocus";
    public const string Form = "form";
    public const string FormAction = "formaction";
    public const string FormEnctype = "formenctype";
    public const string FormMethod = "formmethod";
    public const string FormNoValidate = "formnovalidate";
    public const string FormTarget = "formtarget";
    public const string MaxLength = "maxlength";
    public const string MinLength = "minlength";
    public const string Pattern = "pattern";
    public const string Size = "size";
    public const string Step = "step";
    public const string Min = "min";
    public const string Max = "max";

    // Link and navigation
    public const string Href = "href";
    public const string Target = "target";
    public const string Rel = "rel";
    public const string Download = "download";

    // Media attributes
    public const string Src = "src";
    public const string Alt = "alt";
    public const string Width = "width";
    public const string Height = "height";
    public const string Controls = "controls";
    public const string AutoPlay = "autoplay";
    public const string Loop = "loop";
    public const string Muted = "muted";
    public const string Poster = "poster";
    public const string Preload = "preload";

    // Table attributes
    public const string ColSpan = "colspan";
    public const string RowSpan = "rowspan";
    public const string Headers = "headers";
    public const string Scope = "scope";

    // Event attributes (common ones)
    public const string OnClick = "onclick";
    public const string OnChange = "onchange";
    public const string OnSubmit = "onsubmit";
    public const string OnLoad = "onload";
    public const string OnFocus = "onfocus";
    public const string OnBlur = "onblur";
    public const string OnKeyDown = "onkeydown";
    public const string OnKeyUp = "onkeyup";
    public const string OnMouseOver = "onmouseover";
    public const string OnMouseOut = "onmouseout";

    private const string NoOpener = "noopener";
    private const string NoReferrer = "noreferrer";

    private static readonly SearchValues<char> CharactersToEscape = SearchValues.Create("&<>\"'");

    #endregion

    #region Constructors

    public HtmlAttributesDictionary() : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public HtmlAttributesDictionary(IDictionary<string, object> dictionary) : base(dictionary,
        StringComparer.OrdinalIgnoreCase)
    {
    }

    public HtmlAttributesDictionary(IEnumerable<KeyValuePair<string, object>> collection) : base(collection,
        StringComparer.OrdinalIgnoreCase)
    {
    }

    #endregion

    #region Class Management

    public HtmlAttributesDictionary AddClass(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
        {
            return this;
        }

        if (ContainsClass(className))
        {
            return this;
        }

        var classes = GetClasses();
        classes.Add(className);
        this[Class] = classes.JoinBy(StringJoinBy.Space);

        return this;
    }

    public HtmlAttributesDictionary RemoveClass(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
        {
            return this;
        }

        if (!ContainsClass(className))
        {
            return this;
        }

        var classes = GetClasses();
        classes.Remove(className);

        if (classes.Count > 0)
        {
            this[Class] = classes.JoinBy(StringJoinBy.Space);
        }
        else
        {
            Remove(Class);
        }

        return this;
    }

    public bool HasClass(string className) => !string.IsNullOrWhiteSpace(className) && ContainsClass(className);

    /// <summary>
    ///     Looks for a class name in the attribute without splitting it into a list first.
    /// </summary>
    private bool ContainsClass(ReadOnlySpan<char> className)
    {
        if (!TryGetValue(Class, out var value))
        {
            return false;
        }

        var classes = value.ToString().AsSpan();
        foreach (var range in classes.SplitBy(StringSplitBy.Space))
        {
            var candidate = classes[range];
            if (candidate.Length > 0 && candidate.SequenceEqual(className))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Builds the value of the class attribute.
    /// </summary>
    /// <returns>
    ///     The class names as one string, or <c>null</c> when no class attribute is set. A caller that
    ///     renders the attribute itself is meant to leave it out entirely on <c>null</c>, rather than
    ///     write an empty one.
    /// </returns>
    public string? BuildClass() => ContainsKey(Class) ? GetClasses().JoinBy(StringJoinBy.Space) : null;

    public List<string> GetClasses()
    {
        if (!TryGetValue(Class, out var value))
        {
            return [];
        }

        return [.. value.ToString()!.SplitBy(StringSplitBy.Space)];
    }

    #endregion

    #region Style Management

    public HtmlAttributesDictionary AddStyle(string property, string value)
    {
        if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(value))
        {
            return this;
        }

        var styles = GetStyles();
        styles[property] = value;

        var styleString = BuildStyleString(styles);
        this[Style] = styleString;

        return this;
    }

    public HtmlAttributesDictionary RemoveStyle(string property)
    {
        if (string.IsNullOrWhiteSpace(property))
        {
            return this;
        }

        var styles = GetStyles();
        if (!styles.Remove(property))
        {
            return this;
        }

        if (styles.Count > 0)
        {
            var styleString = BuildStyleString(styles);
            this[Style] = styleString;
        }
        else
        {
            Remove(Style);
        }

        return this;
    }

    public bool HasStyle(string property) => !string.IsNullOrWhiteSpace(property) && GetStyles().ContainsKey(property);

    /// <summary>
    ///     Builds the value of the style attribute out of the declarations it holds.
    /// </summary>
    /// <returns>
    ///     The declarations as one string, or <c>null</c> when no style attribute is set. A caller that
    ///     renders the attribute itself is meant to leave it out entirely on <c>null</c>, rather than
    ///     write an empty one. Whatever the attribute was given as, this returns it normalized.
    /// </returns>
    public string? BuildStyle()
    {
        if (!ContainsKey(Style))
        {
            return null;
        }

        return BuildStyleString(GetStyles());
    }

    private static string BuildStyleString(Dictionary<string, string> styles)
    {
        var separator = StringJoinBy.Semicolon.GetSeperatorCharacter();
        var builder = new StringBuilder();

        foreach (var style in styles)
        {
            if (builder.Length > 0)
            {
                builder.Append(separator);
            }

            builder.Append(style.Key).Append(": ").Append(style.Value);
        }

        return builder.ToString();
    }

    public Dictionary<string, string> GetStyles()
    {
        if (!TryGetValue(Style, out var value))
        {
            return new(StringComparer.OrdinalIgnoreCase);
        }

        var styles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var styleString = value.ToString().AsSpan();

        foreach (var range in styleString.SplitBy(StringSplitBy.Semicolon))
        {
            var declaration = styleString[range];
            var colon = declaration.IndexOf(':');
            if (colon < 0)
            {
                continue;
            }

            var property = declaration[..colon].Trim();
            var styleValue = declaration[(colon + 1)..].Trim();
            if (!property.IsEmpty && !styleValue.IsEmpty)
            {
                styles[property.ToString()] = styleValue.ToString();
            }
        }

        return styles;
    }

    #endregion

    #region Data Attributes Management

    public HtmlAttributesDictionary AddDataAttribute(string name, object value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return this;
        }

        var dataKey = name.StartsWith("data-", StringComparison.OrdinalIgnoreCase)
            ? name
            : $"data-{name}";

        this[dataKey] = value;
        return this;
    }

    public HtmlAttributesDictionary RemoveDataAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return this;
        }

        var dataKey = name.StartsWith("data-", StringComparison.OrdinalIgnoreCase)
            ? name
            : $"data-{name}";

        Remove(dataKey);
        return this;
    }

    public bool HasDataAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        var dataKey = name.StartsWith("data-", StringComparison.OrdinalIgnoreCase)
            ? name
            : $"data-{name}";

        return ContainsKey(dataKey);
    }

    public Dictionary<string, object> GetDataAttributes()
    {
        return this.Where(kvp => kvp.Key.StartsWith("data-", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
    }

    #endregion

    #region Common Attribute Helpers

    public HtmlAttributesDictionary SetId(string id)
    {
        this[Id] = id;
        return this;
    }

    public HtmlAttributesDictionary SetTitle(string title)
    {
        this[Title] = title;
        return this;
    }

    public HtmlAttributesDictionary AddNoAnchorTracking()
    {
        this[Rel] = $"{NoOpener} {NoReferrer}";
        return this;
    }

    public HtmlAttributesDictionary IsReadonly(bool isReadonly = true)
    {
        if (isReadonly)
        {
            this[Readonly] = "readonly";
        }
        else
        {
            Remove(Readonly);
        }

        return this;
    }

    public HtmlAttributesDictionary IsDisabled(bool isDisabled = true)
    {
        if (isDisabled)
        {
            this[Disabled] = "disabled";
        }
        else
        {
            Remove(Disabled);
        }

        return this;
    }

    public HtmlAttributesDictionary IsRequired(bool isRequired = true)
    {
        if (isRequired)
        {
            this[Required] = "required";
        }
        else
        {
            Remove(Required);
        }

        return this;
    }

    public HtmlAttributesDictionary IsChecked(bool isChecked = true)
    {
        if (isChecked)
        {
            this[Checked] = "checked";
        }
        else
        {
            Remove(Checked);
        }

        return this;
    }

    public HtmlAttributesDictionary IsSelected(bool isSelected = true)
    {
        if (isSelected)
        {
            this[Selected] = "selected";
        }
        else
        {
            Remove(Selected);
        }

        return this;
    }

    public HtmlAttributesDictionary IsHidden(bool isHidden = true)
    {
        if (isHidden)
        {
            this[Hidden] = "hidden";
        }
        else
        {
            Remove(Hidden);
        }

        return this;
    }

    public HtmlAttributesDictionary SetPlaceholder(string placeholder)
    {
        this[Placeholder] = placeholder;
        return this;
    }

    public HtmlAttributesDictionary SetValue(object value)
    {
        this[Value] = value;
        return this;
    }

    public HtmlAttributesDictionary SetName(string name)
    {
        this[Name] = name;
        return this;
    }

    public HtmlAttributesDictionary SetType(string type)
    {
        this[Type] = type;
        return this;
    }

    public HtmlAttributesDictionary SetHref(string href)
    {
        this[Href] = href;
        return this;
    }

    public HtmlAttributesDictionary SetSrc(string src)
    {
        this[Src] = src;
        return this;
    }

    public HtmlAttributesDictionary SetAlt(string alt)
    {
        this[Alt] = alt;
        return this;
    }

    public HtmlAttributesDictionary SetTarget(string target)
    {
        this[Target] = target;
        return this;
    }

    public HtmlAttributesDictionary SetTabIndex(int tabIndex)
    {
        this[TabIndex] = tabIndex;
        return this;
    }

    #endregion

    #region Utility Methods

    public string ToHtmlString()
    {
        if (Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (var kvp in this)
        {
            if (sb.Length > 0)
            {
                sb.Append(' ');
            }

            sb.Append(kvp.Key).Append("=\"");
            AppendHtmlEncoded(sb, kvp.Value.ToString());
            sb.Append('"');
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Appends the value with the five characters that cannot stand in an attribute written out, in one pass and
    ///     without a string for each replacement.
    /// </summary>
    private static void AppendHtmlEncoded(StringBuilder builder, ReadOnlySpan<char> value)
    {
        int index;
        while ((index = value.IndexOfAny(CharactersToEscape)) >= 0)
        {
            builder.Append(value[..index]);
            builder.Append(value[index] switch
            {
                '&' => "&amp;",
                '<' => "&lt;",
                '>' => "&gt;",
                '"' => "&quot;",
                _ => "&#39;",
            });

            value = value[(index + 1)..];
        }

        builder.Append(value);
    }

    public HtmlAttributesDictionary Merge(HtmlAttributesDictionary? other)
    {
        if (other == null)
        {
            return this;
        }

        foreach (var kvp in other)
        {
            // Special handling for class attribute - merge classes
            if (string.Equals(kvp.Key, Class, StringComparison.OrdinalIgnoreCase))
            {
                var classesToAdd = kvp.Value.ToString().AsSpan();
                foreach (var range in classesToAdd.SplitBy(StringSplitBy.Space))
                {
                    var className = classesToAdd[range];
                    if (!className.IsEmpty)
                    {
                        AddClass(className.ToString());
                    }
                }
            }
            // Special handling for style attribute - merge styles
            else if (string.Equals(kvp.Key, Style, StringComparison.OrdinalIgnoreCase))
            {
                var otherStyles = new HtmlAttributesDictionary { [Style] = kvp.Value }.GetStyles();
                foreach (var style in otherStyles)
                {
                    AddStyle(style.Key, style.Value);
                }
            }
            else
            {
                this[kvp.Key] = kvp.Value;
            }
        }

        return this;
    }

    #endregion
}
