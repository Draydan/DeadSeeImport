use [DeadSeaCatalogueDAL.ProductContext]

select title from Products p
order by title

select title, c.Name 
from Products p
inner join LinkProductWithCategories l on l.product_ID = p.ID
inner join Categories c on c.ID = l.category_ID
order by title

select * from Categories
order by name