use [DeadSeaCatalogueDAL.ProductContext]
--select distinct p.title, t.title, ca.title, tc.title, ca.titleRus, * from products p
--left outer join Translations t on t.titleeng = p.title
--inner join LinkProductWithCategories lpc on lpc.product_ID = p.ID
--inner join Categories ca on ca.id = lpc.category_ID
--left outer join Translations tc on tc.titleEng = ca.title 
--where 
--'Canaan Body Peeling Soap, Dead Sea Products'
--in (p.title, t.title)
--and tc.isOurCategory = 1


--delete from LinkProductWithCategories where product_ID in (select id from Products where title = 'Mogador Nurturing Eye Cream, Argan Oil')

select p.title, ca.title, * from Products p
inner join LinkProductWithCategories li on li.product_ID = p.id
inner join Categories ca on ca.id = li.category_ID
where p.title = 'Mogador Nurturing Eye Cream, Argan Oil'