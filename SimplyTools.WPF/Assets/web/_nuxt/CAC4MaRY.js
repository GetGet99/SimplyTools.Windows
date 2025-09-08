const n=`{%- meta -%}\r
name: INotifyPropertyChanged Generator\r
description: Generate INotifyPropertyChanged class\r
lang: csharp\r
properties:\r
  # Whether to use C# 14 \`field\` keyword rather than generating backing field\r
  fieldKeyword: boolean\r
  # Whether to include the partial keyword (default: false)\r
  partial: boolean\r
  # List of using namespaces\r
  usings: string[]\r
  # Class name\r
  name: string\r
  # Properties to generate\r
  props:\r
    type: string\r
    name: string\r
example:\r
  namespace: SimplyTools.Example\r
  name: Person\r
  partial: true\r
  fieldKeyword: false\r
  usings:\r
    - System\r
  props:\r
    - type: string\r
      name: FirstName\r
    - type: string\r
      name: LastName\r
    - type: int\r
      name: Age\r
{%- endmeta -%}\r
{%- for using in usings -%}\r
    using {{using}};\r
{% endfor -%}\r
{%- if namespace -%} namespace {{ namespace }} { {%- endif %}\r
    {% if partial %}partial {% endif %}class {{name}} : INotifyPropertyChanged {\r
        {%- for prop in props %}\r
        {% unless fieldKeyword -%}\r
            private {{ prop.type }} _{{ prop.name }};\r
        {% assign fieldname = "_" | append: prop.name -%}\r
        {% else %}\r
            {%- assign fieldname = "field" -%}\r
        {%- endunless -%}\r
        public {{ prop.type }} {{ prop.name }} {\r
            get {\r
                return {{ fieldname }};\r
            }\r
            set {\r
                {{ fieldname }} = value;\r
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof({{prop.name}})))\r
            }\r
        }\r
        {% endfor %}\r
    }\r
{% if namespace -%} } {%- endif -%}`;export{n as default};
