#! /usr/bin/env pro
#QT = core gui widgets
QT =
CONFIG -= QT

CONFIG += c++17 cmdline

TEMPLATE  = lib
#CONFIG += staticlib
CONFIG += dll

SOURCES += \
    dllmain.cpp

#DESTDIR = $$PWD

INCLUDEPATH += $$(HOME)/common/include

HEADERS += \
    dllmain.h

DEFINES += BUILDING_DLL1
