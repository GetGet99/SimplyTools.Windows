const e=`name: INotifyPropertyChanged Generator\r
description: Generate INotifyPropertyChanged class\r
lang: csharp\r
properties:\r
  # Class name\r
  name: string\r
  # Properties to generate\r
  props:\r
    type: string\r
    name: string\r
  # Whether to use C# 14 \`field\` keyword rather than generating backing field\r
  fieldKeyword: boolean\r
  # Whether to include the partial keyword (default: false)\r
  partial: boolean\r
  # List of using namespaces\r
  usings: string[]\r
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
      name: Age`;export{e as default};
