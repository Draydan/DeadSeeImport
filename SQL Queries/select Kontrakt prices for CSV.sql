use [DeadSeaCatalogueDAL.ProductContext]

delete from Products where title = '������������ '

-- ������� ������������ ���� �� ������� ����� ��������������� � ������� �����
-- �.�. ���� ����� ��� ������ �� 100�, � ��������������� ���� 200�, � ������� 0.2
-- �� �� ������ ���� 120� � �������� 20� �������
declare @nacenka float = 0.2
--�������� ������ - ������ ������ �� ������ ����. ���������� ������� ������ �������, �� �� ������� ��� ��� 0.35-0.4 ������� �������� 0.2-0.25. �� ���
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
	when p.details <> '' then '<br>������<br>' + p.details + '. <br>'
	else ''
end
+'<br><br>  �� ����� ����� �� ������ ������ "'+p.title+
'" �� ����� �������� �� ����� ����.  <br><br><br> ����� � ��� �� ������ ������ ������ �������� ����������� ��������� ��������� "'
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
--and p.title like '������� �����������%' --and p.detailsrus like '%������%'
--and p.title like '%���������%'
--and [desc] like '%;%'
--and p.[desc] = ''
--and p.title like '%���� ��� ����%'
--order by ca.title