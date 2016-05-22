use [DeadSeaCatalogueDAL.ProductContext]

select title, titleRus, descrus, detailsRus, [desc], details from Products p
order by title

select * from Categories
order by title

select p.title, c.title, c.id
from Products p
inner join LinkProductWithCategories l on l.product_ID = p.ID
inner join Categories c on c.ID = l.category_ID
order by c.title

select t.title, t.[desc], p.artikul, 
70 * CONVERT(float,replace(p.price, '$','')) as price, 
'http://www.israel-catalog.com/sites/default/files/products/images/' + p.imageFileName as imageFileName
from Products p
inner join Translations t on t.titleEng = p.title
--inner join LinkProductWithCategories l on l.product_ID = p.ID
--inner join Categories c on c.ID = l.category_ID
order by t.title

use [DeadSeaCatalogueDAL.ProductContext]
select * from Products
where title like '%Olive Oil%'