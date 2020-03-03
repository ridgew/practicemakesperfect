#! /usr/bin/python
# coding:gb2312
"""
@author:Ridge Wang
@time:2018/03/31
"""
import os
import time
import datetime
from selenium import webdriver  



global refreshCount
#引入chromedriver.exe
chromedriver = os.path.dirname(__file__) + "./chromedriver.exe"
os.environ["webdriver.chrome.driver"] = chromedriver


refreshCount = 0
myUserName = len(os.sys.argv) > 1 and  os.sys.argv[1] or '上网用户名'
myPassword = len(os.sys.argv) > 2 and  os.sys.argv[2] or '上网密码'

def loginChrome():
    global refreshCount
    browser = webdriver.Chrome(chromedriver)
    url = "http://1.1.1.3/ac_portal/default/pc.html?tabs=pwd"  
    browser.get(url)
    browser.find_element_by_id("password_name").send_keys(myUserName)
    browser.find_element_by_id("password_pwd").send_keys(myPassword)
    cbxChk = browser.find_element_by_id("rememberPwd")
    cbxChk.click()
    browser.find_element_by_id("password_submitBtn").click() 
    refreshCount += 1
    time.sleep(5)
    browser.quit()

def loginChrome2():
    global refreshCount
    browser = webdriver.Chrome(chromedriver)
    url = "http://1.1.1.3/ac_portal/20171031100610/pc.html?template=20171031100610&tabs=pwd&vlanid=0&_ID_=0&switch_url=&url=http://www.baidu.com"  
    browser.get(url)
    browser.find_element_by_id("password_name").send_keys(myUserName)
    browser.find_element_by_id("password_pwd").send_keys(myPassword)
    cbxChk = browser.find_element_by_id("rememberPwd")
    cbxChk.click()
    #browser.find_element_by_id("password_disclaimer").click()
    browser.find_element_by_id("password_submitBtn").click() 
    refreshCount += 1
    time.sleep(5)
    browser.quit()

os.system('title 上网保持脚本请勿关闭程序')
lastException = None
while 1 == 1 :
    try:
         loginChrome2()
    except Exception as e:
        lastException = e
    finally:
        start = datetime.datetime.now()
        nextLoginTime = start + datetime.timedelta(seconds=+3600)
        os.system('cls')
        if lastException != None :
            print(lastException)
        print('第' + str(refreshCount) + '次上网登录..')
        print ('下次执行' + str(nextLoginTime))
        time.sleep(3600)
quit = input('按任意键退出.')
