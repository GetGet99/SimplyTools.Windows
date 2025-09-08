const n=`{%- meta -%}\r
name: Dependency Property Generator\r
description: Generate dependency properties, commonly used for platforms like WPF, WinUI, and UWP.\r
lang: csharp\r
properties:\r
  partial: boolean\r
  usings: string[]\r
  DependencyPropertyClassName: string\r
  name: string\r
  props:\r
    type: string\r
    name: string\r
example:\r
  namespace: SimplyTools.Example\r
  name: Person\r
  partial: true\r
  DependencyPropertyClassName: DependencyProperty\r
  usings:\r
    - Microsoft.UI.Xaml\r
    - Windows.UI.Xaml\r
  props:\r
    - type: string\r
      name: FirstName\r
    - type: string\r
      name: LastName\r
    - type: int\r
      name: Age\r
{%- endmeta -%}\r
{%- unless DependencyPropertyClassName -%}\r
    {% assign DependencyPropertyClassName = "DependencyProperty" %}\r
{%- endunless -%}\r
{%- for using in usings -%}\r
    using {{using}};\r
{% endfor -%}\r
{%- if namespace -%} namespace {{ namespace }} { {%- endif %}\r
    {% if partial %}partial {% endif %}class {{name}} {\r
        {%- for prop in props %}\r
        public static readonly {{ DependencyPropertyClassName }} {{prop.name}}Property =\r
            {{ DependencyPropertyClassName }}.Register(\r
                {{prop.name}},\r
                typeof({{prop.type}}),\r
                typeof({{name}}),\r
                null\r
            );\r
\r
        public {{prop.type}} {{prop.name}}\r
        {\r
            get {\r
                return ({{prop.type}})GetValue({{prop.name}}Property);\r
            }\r
            set {\r
                SetValue({{prop.name}}Property, value);\r
            }\r
        }\r
        {% endfor %}\r
    }\r
{% if namespace -%} } {%- endif -%}`;export{n as default};
