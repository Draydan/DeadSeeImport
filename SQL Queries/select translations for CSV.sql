use [DeadSeaCatalogueDAL.ProductContext]

-- выдача товаров для сайта после 2017.02.18 по израильским теговым категориям
select distinct --p.title, t.titleEng, 
replace(isnull(t.title, p.title), '&amp;','and') as title, 
   CASE 
      WHEN ca.isOurCategory = 0 THEN 'Расширенный список->' + isnull(tc.title, ca.titlerus)
      WHEN ca.isOurCategory = 1 THEN isnull(tc.title, ca.titleRus)
   END as catcat,
isnull(dbo.replacenewline(t.[desc], '<br>'), p.[desc]) [desc], 
'publish' as post_status,
'no' as manage_stock , 'instock' as stock_status, p.artikul, 

-- берем меньшую из оптовой и розничной и добавляем 10%
round(60 * 
case 
 when dbo.b2n(price) < dbo.b2n(pricefull) then dbo.b2n(price) * 1.1
 when dbo.b2n(price) >= dbo.b2n(pricefull) then dbo.b2n(pricefull) * 1.1
end
, -1)
as price,

replace( replace( p.imageFileName, '_003', ''), '_002', '') as imageFileName,
p.imageFileName as product_gallery
from Products p
left outer join Translations t on t.titleEng = p.title and t.title <> p.title
inner join LinkProductWithCategories lpc on lpc.product_ID = p.id  
inner join Categories ca on ca.ID = lpc.category_ID 
left outer join Translations tc on tc.titleEng = ca.title 

--where p.title = 'Active Facial Toner Enriched With Dead Sea Minerals'
order by replace(isnull(t.title, p.title), '&amp;','and')