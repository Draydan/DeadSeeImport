use [DeadSeaCatalogueDAL.ProductContext]

delete from Products where title = 'Наименование '

-- наценка составляющая долю от разницы между рекомендованной и оптовой ценой
-- т.е. если товар нам выдают по 100р, а рекомендованная цена 200р, и наценка 0.2
-- то мы ставим цену 120р и получаем 20р прибыли
declare @nacenka float = 0.2
--обратный подход - делаем скидку от полной цены. Рассчитать прибыль теперь сложнее, но со скидкой для нас 0.35-0.4 выходит примерно 0.2-0.25. Хм нда
declare @skidka float = 0.15

select 
distinct p.title, 
--'Aphrodite, Health & Beauty, Dr.Sea, Shemen Amour->' + 
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
round(dbo.b2n(price) + (dbo.b2n(priceFull)-dbo.b2n(price)) * @nacenka , -1) priceOld,
case
	when ca.title = 'Health & Beauty' then round(dbo.b2n(priceFull) * (1 - @skidka - 0.05), -1) 
	else round(dbo.b2n(priceFull) * (1 - @skidka), -1)
end  priceNew, price, pricefull
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
inner join Categories ca2 on ca2.id = li2.category_ID and ca2.title in 
--('Health & Beauty')
('Shemen Amour', 'Health & Beauty', 'Dr.Sea', 'Aphrodite')

where p.supplier_ID = 2
--and p.title like 'Бальзам осветляющий%' --and p.detailsrus like '%основе%'
--and p.title like '%Сыворотка%'
--and [desc] like '%;%'
--and p.[desc] = ''
--and p.title like '%Гель для лица%'
--order by ca.title