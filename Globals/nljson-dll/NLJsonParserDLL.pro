#! /usr/bin/env pro
#QT = core gui widgets
QT =
CONFIG -= QT

CONFIG += c++17 cmdline

TEMPLATE  = lib
#CONFIG += staticlib
CONFIG += dll

SOURCES += \
    NLJsonParser_wrap.cpp \
    nljson.cpp

#DESTDIR = $$PWD

INCLUDEPATH += $$(HOME)/common/include

HEADERS += \
    NLJsonParser_wrap.h \
    nljson.h

DEFINES += BUILDING_DLL1
