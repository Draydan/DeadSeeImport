delete from Products where title = 'Наименование '

select 
distinct p.title, 
'Aphrodite, Heath & Beauty, Dr.Sea, Shemen Amour->' + 
isnull(ca.title, ca.titlerus), 
p.[desc]

+ '<br>Состав<br>' + p.details
+'. <br><br><br>  На нашем сайте вы можете купить "'+p.title+
'" напрямую из Израиля по самой выгодной на рынке цене.  <br><br><br> Также у нас вы можете купить другие продукты израильской косметики категории "'
+ isnull(ca.title, ca.titlerus) + '"' 
, 'publish', 'no','instock', artikul, 
round(dbo.b2n(price) + (dbo.b2n(priceFull)-dbo.b2n(price))/5*1 , 0)
 imageFileName, imageFileName 

from products p
inner join LinkProductWithCategories li on li.product_ID = p.id
inner join Categories ca on ca.id = li.category_ID
where p.supplier_ID = 2
--and p.[desc] = ''
--and p.title like '%Гель для лица%'
--order by ca.title