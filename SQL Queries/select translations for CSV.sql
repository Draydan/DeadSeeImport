use [DeadSeaCatalogueDAL.ProductContext]

-- ������ ������� ��� ����� ����� 2017.02.18 �� ����������� ������� ����������
declare @nakrutka float = 1.1

select distinct --p.title, t.titleEng, 

replace(isnull(t.title, p.title), '&amp;','and') as title, 

CASE 
      WHEN ca.isOurCategory = 0 THEN '����������� ������->' + isnull(tc.title, ca.titlerus)
      WHEN ca.isOurCategory = 1 THEN isnull(tc.title, ca.titleRus)
   END as catcat,

replace(isnull(dbo.replacenewline(t.[desc], '<br>'), p.[desc]), ';','') 
+'. <br><br><br>  �� ����� ����� �� ������ ������ "'+replace(isnull(t.title, p.title), '&amp;','and')+
'" �������� �� ������� �� ����� �������� �� ����� ����.  <br><br><br> ����� � ��� �� ������ ������ ������ �������� ����������� ��������� ��������� "'+
   CASE 
      WHEN ca.isOurCategory = 0 THEN isnull(tc.title, ca.titlerus)
      WHEN ca.isOurCategory = 1 THEN isnull(tc.title, ca.titleRus)
   END
   + case 
		when t.title is null then '"'
		when t.title is not null then '" <br><br><br> ������������� �������� ������ - "' + p.title + '"'
	end
as [desc], 

'publish' as post_status,
'no' as manage_stock , 'instock' as stock_status, p.artikul, 

-- ����� ������� �� ������� � ��������� � ��������� 10%
round(60 * 
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
order by replace(isnull(t.title, p.title), '&amp;','and')