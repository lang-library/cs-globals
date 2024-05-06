#ifndef DLLMAIN_H
#define DLLMAIN_H

//#include <QtCore>
#include <windows.h>

#if 0x0
class API: public QObject
{
    Q_OBJECT
public:
    //launch_process_thread *thread = new launch_process_thread();
public:
    API()
    {
        qDebug() << "API thread" << QThread::currentThreadId();
        //thread->start();
    }
    virtual ~API()
    {
    }
    Q_INVOKABLE QVariant add2(QVariant args)
    {
        auto list = args.toList();
        if (list.length() != 2) return QVariant();
        auto answer = list[0].toDouble() + list[1].toDouble();
        return QVariantList() << answer;
    }
    Q_INVOKABLE QVariant div2(QVariant args)
    {
        auto list = args.toList();
        if (list.length() != 2) return "div2() takes 2 double arguments";
        if (list[1].toDouble()==0) return "div2(): division by 0";
        auto answer = list[0].toDouble() / list[1].toDouble();
        return QVariantList() << answer;
    }
};
#endif

#endif // DLLMAIN_H
