#! /usr/bin/env pro
#QT = core gui widgets
QT =
CONFIG -= QT

CONFIG += c++17 cmdline

TEMPLATE  = lib
#CONFIG += staticlib
CONFIG += dll

SOURCES += \
    PegParser_wrap.cpp \
    pegparser.cpp

#DESTDIR = $$PWD

INCLUDEPATH += $$(HOME)/common/include

HEADERS += \
    pegparser.h

DEFINES += BUILDING_DLL1
