use [DeadSeaCatalogueDAL.ProductContext]

--select distinct --p.title, t.titleEng, 
--t.title, tc.title,  dbo.replacenewline(t.[desc], '<br>') [desc], 
--'no' as manage_stock , 'instock' as stock_status, p.artikul, 
--65 * CONVERT(float,replace(p.price, '$','')) as price, 
----'http://www.israel-catalog.com/sites/default/files/products/images/' + p.imageFileName as imageFileName
----'http://izrael-cosmetics.ru/product_gallery/' + p.imageFileName as imageFileName
----'/product_gallery/' + p.imageFileName as imageFileName
----'http://izrael-cosmetics.ru.xsph.ru/wp-content/uploads/2016/05/' + p.imageFileName as imageFileName
--p.imageFileName as imageFileName
--from Products p
--inner join Translations t on t.titleEng = p.title
--inner join LinkProductWithCategories lpc on lpc.product_ID = p.id  
--inner join Categories ca on ca.ID = lpc.category_ID 
--inner join Translations tc on tc.titleEng = ca.title and tc.isOurCategory = 1
--order by t.title


select distinct --p.title, t.titleEng, 
isnull(t.title, p.title), ca.titleRus, isnull(dbo.replacenewline(t.[desc], '<br>'), p.[desc]) [desc], 
'publish' as post_status,
'no' as manage_stock , 'instock' as stock_status, p.artikul, 
-- расчет цены идет между скидкой xx для 450р товара и скидкой 30% для 3300р
round(65 * CONVERT(float,replace(p.price, '$','')) * (0.2 * (3800-65 * CONVERT(float,replace(p.price, '$','')))/ 3300 +0.7), -1) as price, 
--65 * CONVERT(float,replace(p.price, '$','')),
--'http://www.israel-catalog.com/sites/default/files/products/images/' + p.imageFileName as imageFileName
--'http://izrael-cosmetics.ru/product_gallery/' + p.imageFileName as imageFileName
--'/product_gallery/' + p.imageFileName as imageFileName
--'http://izrael-cosmetics.ru.xsph.ru/wp-content/uploads/2016/05/' + p.imageFileName as imageFileName
p.imageFileName as imageFileName
from Products p
left outer join Translations t on t.titleEng = p.title and t.title <> p.title
inner join LinkProductWithCategories lpc on lpc.product_ID = p.id  
inner join Categories ca on ca.ID = lpc.category_ID 
inner join Translations tc on tc.title = ca.titleRus and tc.isOurCategory = 1
--order by t.title

select * from
[DeadSeaCatalogueDAL.ProductContext].dbo.Translations t
where titleEng = '' or isnull(titleEng, '')=''
and not exists 
(select * from Translations t2 where t.title = t2.title and t2.titleEng is not null)
order by t.title

select t.title, t.[desc], p.title, p.[desc],p.details from [DeadSeaCatalogueDAL.ProductContext].dbo.Products p
left outer join [DeadSeaCatalogueDAL.ProductContext].dbo.Translations t on t.titleEng = p.title

select count(t.id), p.title from [DeadSeaCatalogueDAL.ProductContext].dbo.Products p
left outer join [DeadSeaCatalogueDAL.ProductContext].dbo.Translations t on t.titleEng = p.title
group by p.title
order by count(t.id)

select distinct t.title
from Translations t
order by title

select count(*), tc.title
from LinkProductWithCategories lpc 
inner join Categories ca on ca.ID = lpc.category_ID 
inner join Translations tc on tc.titleEng = ca.title and ca.isOurCategory = 1 and tc.isOurCategory = 1
group by tc.title

select * from Categories ca
inner join LinkProductWithCategories li on li.category_ID = ca.ID
inner join Products p on p.id = li.product_ID
inner join Translations t on t.title = ca.titleRus
where ca.titleRus = 'для жирной кожи'

select distinct tc.titleEng, count(*)
from Products p
inner join Translations t on t.titleEng = p.title and t.title <> p.title
inner join LinkProductWithCategories lpc on lpc.product_ID = p.id  
inner join Categories ca on ca.ID = lpc.category_ID 
inner join Translations tc on tc.title = ca.titleRus and tc.isOurCategory = 1
group by tc.titleEng
order by tc.titleEng


select tc.titleEng, count(*)
from Products p
inner join Translations t on t.titleEng = p.title and t.title <> p.title
inner join LinkProductWithCategories lpc on lpc.product_ID = p.id  
inner join Categories ca on ca.ID = lpc.category_ID 
inner join Translations tc on tc.title = ca.titleRus and tc.isOurCategory = 1
group by tc.titleEng
order by tc.titleEng

select tc.title, count(*)
from Products p
inner join Translations t on t.titleEng = p.title
inner join LinkProductWithCategories lpc on lpc.product_ID = p.id  
inner join Categories ca on ca.ID = lpc.category_ID 
inner join Translations tc on tc.titleEng = ca.title and tc.isOurCategory = 1
group by tc.title
order by tc.title


select t.title, tc.title,  ca.title, tc.keyWords, tc.antiKeyWords, p.[desc], p.details
from Products p
inner join Translations t on t.titleEng = p.title
inner join LinkProductWithCategories lpc on lpc.product_ID = p.id  
inner join Categories ca on ca.ID = lpc.category_ID 
inner join Translations tc on tc.titleEng = ca.title and tc.isOurCategory = 1
--inner join Translations tc on tc.title = ca.titleRus-- and tc.isOurCategory = 1
order by t.title

select * from [DeadSeaCatalogueDAL.ProductContext].dbo.products
where
--[desc] like '%moisturizing cream%'
--or details like '%moisturizing cream%' or
 artikul = 15867