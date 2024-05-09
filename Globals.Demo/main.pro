#! /usr/bin/env pro
#QT = core
CONFIG -= QT

#TARGET = {{appname}}

CONFIG += c++17 cmdline
#CONFIG += cmdline

#TEMPLATE  = lib
#CONFIG += staticlib
#CONFIG += dll

SOURCES += \
        main.cpp

#HEADERS +=

#DESTDIR = $$PWD

INCLUDEPATH += $$(HOME)/common/include

LIBS += 
