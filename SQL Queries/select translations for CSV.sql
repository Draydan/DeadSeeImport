use [DeadSeaCatalogueDAL.ProductContext]

select distinct --p.title, t.titleEng, 
t.title, tc.title,  dbo.replacenewline(t.[desc], '<br>') [desc], 
'no' as manage_stock , 'instock' as stock_status, p.artikul, 
65 * CONVERT(float,replace(p.price, '$','')) as price, 
--'http://www.israel-catalog.com/sites/default/files/products/images/' + p.imageFileName as imageFileName
--'http://izrael-cosmetics.ru/product_gallery/' + p.imageFileName as imageFileName
--'/product_gallery/' + p.imageFileName as imageFileName
--'http://izrael-cosmetics.ru.xsph.ru/wp-content/uploads/2016/05/' + p.imageFileName as imageFileName
p.imageFileName as imageFileName
from Products p
inner join Translations t on t.titleEng = p.title
inner join LinkProductWithCategories lpc on lpc.product_ID = p.id  
inner join Categories ca on ca.ID = lpc.category_ID 
inner join Translations tc on tc.titleEng = ca.title and tc.isOurCategory = 1
order by t.title

select * from
[DeadSeaCatalogueDAL.ProductContext].dbo.Translations t
where titleEng = '' or isnull(titleEng, '')=''
and not exists 
(select * from Translations t2 where t.title = t2.title and t2.titleEng is not null)
order by t.title

select t.title, t.[desc], p.title, p.[desc],p.details from Products p
left outer join Translations t on t.titleEng = p.title

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
where ca.titleRus = 'лечебная косметика'