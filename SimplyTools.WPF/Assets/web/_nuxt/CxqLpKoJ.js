const r=`{%- meta -%} \r
name: 'Example: Orders'\r
description: Example Snippet\r
properties:\r
  customer_name: string\r
  order_items:\r
    - name: string\r
      price: number\r
  total_price: number\r
example:\r
  customer_name: Alice\r
  total_price: 42\r
  order_items:\r
    - name: "Book"\r
      price: 12\r
    - name: "Pen"\r
      price: 5\r
    - name: "Bag"\r
      price: 25\r
{%- endmeta -%}\r
\r
Hello {{ customer_name }}!\r
Your order total is \${{ total_price }}.\r
{% if order_items %}\r
Items:\r
{%- for item in order_items %}\r
- {{ item.name }} (\${{ item.price }})\r
{%- endfor -%}\r
{% endif %}`;export{r as default};
