delete from Products where title = '������������ '

select 
distinct p.title, 
'Aphrodite, Heath & Beauty, Dr.Sea, Shemen Amour->' + 
isnull(ca.title, ca.titlerus), 
p.[desc]

+ '<br>������<br>' + p.details
+'. <br><br><br>  �� ����� ����� �� ������ ������ "'+p.title+
'" �������� �� ������� �� ����� �������� �� ����� ����.  <br><br><br> ����� � ��� �� ������ ������ ������ �������� ����������� ��������� ��������� "'
+ isnull(ca.title, ca.titlerus) + '"' 
, 'publish', 'no','instock', artikul, 
round(dbo.b2n(price) + (dbo.b2n(priceFull)-dbo.b2n(price))/5*1 , 0)
 imageFileName, imageFileName 

from products p
inner join LinkProductWithCategories li on li.product_ID = p.id
inner join Categories ca on ca.id = li.category_ID
where p.supplier_ID = 2
--and p.[desc] = ''
--and p.title like '%���� ��� ����%'
--order by ca.title