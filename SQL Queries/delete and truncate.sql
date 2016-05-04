use [DeadSeaCatalogueDAL.ProductContext]
truncate table products
truncate table categories
delete from Categories

delete from LinkProductWithCategories where id in
(select li.id from LinkProductWithCategories li where exists
(select * from LinkProductWithCategories li2 
where li2.id < li.id
and li.category_ID = li2.category_ID and li.product_ID = li2.product_ID))