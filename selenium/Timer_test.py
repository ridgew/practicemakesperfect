#! /usr/bin/python
# coding:utf-8
"""
@author:Administrator
@file:Timer_test.py
@time:2018/02/08
https://www.cnblogs.com/Detector/p/8433830.html
"""
import requests
import re
from threading import Timer


def views_count():
    global count
    global source_view
    article_views = []
    url = "http://www.cnblogs.com/Detector/default.html?page=%s"
    for i in range(1, 5):
        html = requests.get(url % i).text
        article_view = re.findall("_Detector 阅读\((.*?)\)", html)
        article_views += article_view
    count += 1
    current_view = sum(map(lambda x: int(x), article_views))
    if current_view - source_view > 50:
        print("You have made great progress")
    else:
        print("current_view: ", current_view)
    if count < 10:  # 运行一万次
        Timer(10, views_count).start()


count = 0
source_view = 2412  # 设定一个初始阅读数据
Timer(10, views_count).start()