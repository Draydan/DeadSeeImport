use [DeadSeaCatalogueDAL.ProductContext]

select p.title prod, ca.title cat, ca.titlerus catrus, catr.title ctran, catr.id ctranid, * from Products p 
inner join LinkProductWithCategories lip on lip.product_ID = p.id
inner join Categories ca on ca.id = lip.category_ID
left outer join Translations catr on catr.titleEng = ca.title
where --ca.title like '%perfume%' and 
p.title like '%Dead Sea Spa Cosmetics Original Eye Cream%'

select count (*) , p.title, isnull(ca.title, ca.titlerus) from Products p 
inner join LinkProductWithCategories lip on lip.product_ID = p.id
inner join Categories ca on ca.id = lip.category_ID
group by p.title, isnull(ca.title, ca.titlerus) 