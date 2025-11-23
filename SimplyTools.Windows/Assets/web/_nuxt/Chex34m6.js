const e=`name: Dependency Property Generator\r
description: Generate dependency properties, commonly used for platforms like WPF, WinUI, and UWP.\r
lang: csharp\r
properties:\r
  # Class name\r
  name: string\r
  namespace: string\r
  # Properties to generate\r
  props:\r
    type: string\r
    name: string\r
  usings: string[]\r
  # DependencyProperty class name (defaults to "DependencyProperty")\r
  DependencyPropertyClassName: string\r
  # Whether to include the partial keyword (default: false)\r
  partial: boolean\r
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
      name: Age`;export{e as default};
