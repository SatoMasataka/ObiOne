# -*- coding: sjis -*-
import xlrd
import glob
import sqlite3
import datetime

def DeleteTable(conn,targetTab):
    cur = conn.execute("SELECT * FROM sqlite_master WHERE type='table' and name='%s'" % targetTab)
    if cur.fetchone() != None:  #存在していたら削除
        conn.execute("DROP TABLE %s" % targetTab)
        
def CreateTables():
    conn = sqlite3.connect('C:/Users/Masataka/Desktop./ObiOneDb.db')
    #DeleteTable(conn,'USER_INFO')
    #DeleteTable(conn,'OBI_INFO')
    DeleteTable(conn,'UPLOAD_INFO')
    DeleteTable(conn,'SHOP_INFO')
    DeleteTable(conn,'SHOP_OBI')
    
    #conn.execute("CREATE TABLE USER_INFO(ID INTEGER primary key autoincrement, NAME TEXT,REGIST_DATE TEXT, DELETE_FLG INTEGER,GOOGLE_ID TEXT)")
    #conn.execute("CREATE TABLE OBI_INFO(OBI_ID INTEGER primary key autoincrement, ID INTEGER, IMG_PATH TEXT,BOOK_TITLE TEXT,AUTHOR TEXT,PUBLISHER TEXT,LINK TEXT,REGIST_DATE TEXT, DELETE_FLG INTEGER)")
    conn.execute("CREATE TABLE UPLOAD_INFO(UPLOAD_ID INTEGER primary key autoincrement, ID INTEGER, UPLOAD_PATH TEXT, DELETE_FLG INTEGER,REGIST_DATE TEXT)")
    conn.execute("CREATE TABLE SHOP_INFO(SHOP_ID TEXT primary key, ID INTEGER, SHOP_NAME TEXT,SHOP_COMMENT TEXT,BACK_IMG TEXT,LAYOUT TEXT,DELETE_FLG INTEGER,REGIST_DATE TEXT)")
    conn.execute("CREATE TABLE SHOP_OBI( OBI_ID TEXT, SHOP_ID TEXT,ORDER_SEQ INTEGER,REGIST_DATE TEXT)")
    
    conn.commit()



if __name__ == "__main__":
    CreateTables()
        

