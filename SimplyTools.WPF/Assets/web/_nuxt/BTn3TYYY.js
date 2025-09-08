const r=`name: 'Example: Orders'\r
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
      price: 25`;export{r as default};
