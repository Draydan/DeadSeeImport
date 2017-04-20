use [DeadSeaCatalogueDAL.ProductContext]
select distinct p.title, t.title, ca.title, tc.title, ca.titleRus, * from products p
left outer join Translations t on t.titleeng = p.title
inner join LinkProductWithCategories lpc on lpc.product_ID = p.ID
inner join Categories ca on ca.id = lpc.category_ID
left outer join Translations tc on tc.titleEng = ca.title 
where 
'Canaan Body Peeling Soap, Dead Sea Products'
in (p.title, t.title)
and tc.isOurCategory = 1