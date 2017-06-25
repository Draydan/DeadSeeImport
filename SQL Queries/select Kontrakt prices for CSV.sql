use [DeadSeaCatalogueDAL.ProductContext]

delete from Products where title = 'Наименование '

select 
distinct p.title, 
'Aphrodite, Heath & Beauty, Dr.Sea, Shemen Amour->' + 
case
	when ca.isourcategory = 1 then isnull(ca.title, ca.titlerus)
	else isnull(ca2.title, ca2.titlerus) + '->' + isnull(ca.title, ca.titlerus)
end,
case 
	when p.[desc] = '' then p.title
	else p.[desc]
end +
case 
	when p.details <> '' then '<br>Состав<br>' + p.details + '. <br>'
	else ''
end
+'<br><br>  На нашем сайте вы можете купить "'+p.title+
'" по самой выгодной на рынке цене.  <br><br><br> Также у нас вы можете купить другие продукты израильской косметики категории "'
+ isnull(ca.title, ca.titlerus) + '"' 
, 'publish', 'no','instock', artikul, 
round(dbo.b2n(price) + (dbo.b2n(priceFull)-dbo.b2n(price))/5*1 , -1) price
, imageFileName, 
replace(imageFileName , '.jpg', '-1.jpg') +','+
replace(imageFileName , '.jpg', '-2.jpg') +','+
replace(imageFileName , '.jpg', '-3.jpg') +','+
replace(imageFileName , '.jpg', '-4.jpg') +','
from products p
inner join LinkProductWithCategories li on li.product_ID = p.id
inner join Categories ca on ca.id = li.category_ID 
and isnull(ca.title, '-') not in ('Shemen Amour', 'Health & Beauty', 'Dr.Sea', 'Aphrodite')

inner join LinkProductWithCategories li2 on li2.product_ID = p.id
inner join Categories ca2 on ca2.id = li2.category_ID and ca2.title in ('Shemen Amour', 'Health & Beauty', 'Dr.Sea', 'Aphrodite')
where p.supplier_ID = 2
--and p.title like '%Крем для волос  увляжняющий с маслом Аргании, 400мл%'
--and [desc] like '%;%'
--and p.[desc] = ''
--and p.title like '%Гель для лица%'
--order by ca.title