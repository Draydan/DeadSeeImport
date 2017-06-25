use [DeadSeaCatalogueDAL.ProductContext]

-- выдача товаров для сайта после 2017.02.18 по израильским теговым категориям
declare @nakrutka float = 1.1
declare @kursBaksa int = 64

select distinct --p.title, t.titleEng, 

replace(isnull(t.title, p.title), '&amp;','and') as title, 

CASE 
      WHEN ca.isOurCategory = 0 THEN 'Расширенный список->' + isnull(tc.title, ca.titlerus)
      WHEN ca.isOurCategory = 1 THEN isnull(tc.title, ca.titleRus)
   END as catcat,

replace(isnull(dbo.replacenewline(t.[desc], '<br>'), p.[desc]), ';','') 
+'. <br><br><br>  На нашем сайте вы можете купить "'+replace(isnull(t.title, p.title), '&amp;','and')+
'" напрямую из Израиля по самой выгодной на рынке цене.  <br><br><br> Также у нас вы можете купить другие продукты израильской косметики категории "'+
   CASE 
      WHEN ca.isOurCategory = 0 THEN isnull(tc.title, ca.titlerus)
      WHEN ca.isOurCategory = 1 THEN isnull(tc.title, ca.titleRus)
   END
   + case 
		when t.title is null then '"'
		when t.title is not null then '" <br><br><br> Международное название товара - "' + p.title + '"'
	end
as [desc], 

'publish' as post_status,
'no' as manage_stock , 'instock' as stock_status, p.artikul, 

-- берем меньшую из оптовой и розничной и добавляем 10%
round(@kursBaksa * 
case 
 when dbo.b2n(price) < dbo.b2n(pricefull) then dbo.b2n(price) * @nakrutka
 when dbo.b2n(price) >= dbo.b2n(pricefull) then dbo.b2n(pricefull) * @nakrutka
end
, -1)
as price,

replace( replace( replace(p.imageFileName, '_003', ''), '_002', ''), '_01', '') as imageFileName,
replace( replace( replace(p.imageFileName, '_003', ''), '_002', ''), '_01', '')+',' +
replace( replace( replace( replace(p.imageFileName, '_003', ''), '_002', ''), '_01', ''), '.jpg', '') + '_01.jpg'
as product_gallery
from Products p
left outer join Translations t on t.titleEng = p.title and t.title <> p.title
inner join LinkProductWithCategories lpc on lpc.product_ID = p.id  
inner join Categories ca on ca.ID = lpc.category_ID 
left outer join Translations tc on tc.titleEng = ca.title 
--where artikul = '15882'
--where p.title = 'Active Facial Toner Enriched With Dead Sea Minerals'
--where p.title like '%lavilin%'
where
CASE 
      WHEN ca.isOurCategory = 0 THEN 'Расширенный список->' + isnull(tc.title, ca.titlerus)
      WHEN ca.isOurCategory = 1 THEN isnull(tc.title, ca.titleRus)
   END 
   is not null
--and tc.title like '%арфюм%'

--and p.title = 'Mogador Nurturing Eye Cream, Argan Oil'
--and p.title in ('Dead Sea Acive Mineral Intense by Natural Sea Beauty',
--'Dead Sea Active Anti-Aging Eye Cream by Natural Sea Beauty',
--'Mineral Lift Renewal Cream by Natural Sea Beauty')
--or p.artikul in ('7622', '13080', '17012')

order by replace(isnull(t.title, p.title), '&amp;','and')