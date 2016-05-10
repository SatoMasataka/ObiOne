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
    DeleteTable(conn,'USER_INFO')
    DeleteTable(conn,'OBI_INFO')
    conn.execute("CREATE TABLE USER_INFO(ID INTEGER primary key autoincrement, NAME TEXT,REGIST_DATE TEXT, DELETE_FLG INTEGER,GOOGLE_ID TEXT)")
    conn.execute("CREATE TABLE OBI_INFO(OBI_ID INTEGER primary key autoincrement, ID INTEGER, IMG_PATH TEXT,BOOK_TITLE TEXT,AUTHOR TEXT,PUBLISHER TEXT,LINK TEXT,REGIST_DATE TEXT, DELETE_FLG INTEGER)")

    conn.commit()



if __name__ == "__main__":
    CreateTables()
    #PrepareTransactTable()
        

