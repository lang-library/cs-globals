#! /usr/bin/env pro
#QT = core gui widgets
QT =
CONFIG -= QT

CONFIG += c++17 cmdline

TEMPLATE  = lib
#CONFIG += staticlib
CONFIG += dll

SOURCES += \
    dllmain.cpp \
    pegparser.cpp

#DESTDIR = $$PWD

INCLUDEPATH += $$(HOME)/common/include

HEADERS += \
    dllmain.h \
    pegparser.h

DEFINES += BUILDING_DLL1
