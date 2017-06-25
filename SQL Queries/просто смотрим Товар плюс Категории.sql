select p.title, ca.title, * from Products p
inner join LinkProductWithCategories li on li.product_ID = p.id
inner join Categories ca on ca.id = li.category_ID
where p.supplier_ID = 2