use [DeadSeaCatalogueDAL.ProductContext]

delete from LinkProductWithCategories where id in
(select li.id from LinkProductWithCategories li where exists
(select * from LinkProductWithCategories li2 
where li2.id < li.id
and li.category_ID = li2.category_ID and li.product_ID = li2.product_ID))

delete from Categories 
where title = 'Super Savers'
and id not in (select li.category_ID from LinkProductWithCategories li)

delete from LinkProductWithCategories

--truncate table categories
delete from Categories

--truncate table products
delete from Products

-- delete where & and And in Translated
delete from translations
where replace(title, 'and', '&') in (select p.title from products p)
or replace(title, ' and ', ' & ') in (select p.title from products p)