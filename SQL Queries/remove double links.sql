use [DeadSeaCatalogueDAL.ProductContext]
-- ������� �� ��1 ���, ��� ���� ��2 ����� ������
delete from LinkProductWithCategories
where id in
(select id from LinkProductWithCategories li1
where exists
(select * from LinkProductWithCategories li2
where li1.category_ID = li2.category_ID and li1.product_ID = li2.product_ID and li2.id < li1.ID))